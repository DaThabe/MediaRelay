using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MediaRelay.Messaging.Queue;


public sealed class UrlMessageQueueConsumer(
        IMessageQueue<UrlMessage, Uri> urlQueue,
        IUrlRelayService urlRelayService,
        IOptions<UrlMessageQueueOptions> options,
        ILogger<UrlMessageQueueConsumer> logger
    ) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("网址处理任务已启动");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var message = await urlQueue.DequeueAsync(stoppingToken);

                using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                timeoutCts.CancelAfter(options.Value.ProcessingTimeout);

                try
                {
                    await urlRelayService.RelayAsync(message.Content, timeoutCts.Token);
                    await urlQueue.AcknowledgeAsync(message.Id, timeoutCts.Token);
                }
                catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested && !stoppingToken.IsCancellationRequested)
                {
                    LogMessageTimeout(message.Id);
                    await urlQueue.RejectAsync(message.Id, stoppingToken);
                }
                catch (Exception ex)
                {
                    await urlQueue.RejectAsync(message.Id, cancellationToken: stoppingToken);
                    LogMessageException(message.Id, ex);
                }
            }
            catch (ObjectDisposedException)
            {
                logger.LogInformation("队列已释放，网址处理任务结束");
                return;
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning("网址处理任务已取消");
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "队列异常");
                await Task.Delay(options.Value.ErrorRetryDelay, stoppingToken);
            }
        }
    }

    private void LogMessageTimeout(MessageId messageId)
    {
        if (!logger.IsEnabled(LogLevel.Warning)) return;

        logger.LogWarning("消息处理超时, MessageId={MessageId}", messageId);
    }
    private void LogMessageException(MessageId messageId, Exception exception)
    {
        if (!logger.IsEnabled(LogLevel.Error)) return;

        logger.LogError(exception, "消息处理失败, MessageId={MessageId}", messageId);
    }
}