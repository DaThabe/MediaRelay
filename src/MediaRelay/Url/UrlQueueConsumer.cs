using MediaRelay.Messaging.Queue;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MediaRelay.Url;


public sealed class UrlQueueConsumer(
        IMessageQueue<UrlMessage, Uri> uriQueue,
        IUrlRelayService urlRelayService,
        ILogger<UrlQueueConsumer> logger
    ) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("网址处理任务已启动");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var message = await uriQueue.DequeueAsync(stoppingToken);
                logger.LogInformation("接收到消息");

                try
                {
                    await urlRelayService.RelayAsync(message.Content, stoppingToken);
                    await uriQueue.AcknowledgeAsync(message, stoppingToken);

                    logger.LogInformation("消息处理完成");
                }
                catch (Exception ex)
                {
                    await uriQueue.RejectAsync(message, cancellationToken: stoppingToken);
                    logger.LogError(ex, "消息处理失败");
                }
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("网址处理任务已取消");
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "队列异常");
            }
        }
    }
}