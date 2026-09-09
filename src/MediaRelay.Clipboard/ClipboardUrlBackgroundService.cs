using MediaRelay.Messaging;
using MediaRelay.Url;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TextCopy;

namespace MediaRelay.Clipboard;


internal sealed class ClipboardUrlBackgroundService(
        IMessageOrchestrator messageOrchestrator,
        IOptions<ClipboardOptions> options,
        ILogger<ClipboardUrlBackgroundService> logger
    ) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("监听任务已启动");

        Uri? lastUri = null;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var text = await ClipboardService.GetTextAsync(stoppingToken);
                var trimmed = text?.Trim();

                if (string.IsNullOrWhiteSpace(trimmed)) continue;

                if (Uri.TryCreate(text, UriKind.Absolute, out var url))
                {
                    if (lastUri == url) continue;

                    using var _ = logger.BeginScope("Url", url);
                    logger.LogInformation("检测到网址");

                    lastUri = url;
                    await messageOrchestrator.SendAsnc<UrlMessage, Uri>(url, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("监听任务已取消");
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "监听任务异常");
            }
            finally
            {
                await Task.Delay(options.Value.PollingInterval, stoppingToken);
            }
        }
    }
}