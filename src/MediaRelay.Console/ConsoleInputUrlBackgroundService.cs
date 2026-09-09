using AsyncConsoleReader;
using MediaRelay.Messaging;
using MediaRelay.Url;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MediaRelay.Console;


internal sealed class ConsoleInputUrlBackgroundService(
        IMessageOrchestrator messageOrchestrator,
        ILogger<ConsoleInputUrlBackgroundService> logger
    ) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("监听任务已启动");

        while (!stoppingToken.IsCancellationRequested)
        {
            System.Console.CursorVisible = true;

            try
            {
                var input = AsyncConsole.ReadLine(stoppingToken)?.Trim();
                if (string.IsNullOrEmpty(input)) continue;

                using var _ = logger.BeginScope("Input", input);

                if (!Uri.TryCreate(input, UriKind.Absolute, out var url))
                {
                    logger.LogWarning("输入不是有效网址");
                    continue;
                }

                await messageOrchestrator.SendAsnc<UrlMessage, Uri>(url, stoppingToken);
            }
            catch(OperationCanceledException)
            {
                logger.LogInformation("监听任务已取消");
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "监听任务异常");
            }
        }
    }
}