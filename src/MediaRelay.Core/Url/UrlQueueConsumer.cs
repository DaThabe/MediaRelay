using MediaRelay.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MediaRelay.Url;


public sealed class UrlQueueConsumer(
        IMessageQueue<InputUrlMessage, Uri> uriQueue,
        IUrlRelayService urlRelayService,
        ILogger<UrlQueueConsumer> logger
    ) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var message = await uriQueue.PeekAsync(stoppingToken);
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
                logger.LogError(ex, "消息处理失败, 已经返回队列");
            }
        }
    }
}