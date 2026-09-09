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
        while (!stoppingToken.IsCancellationRequested)
        {
            System.Console.CursorVisible = true;

            try
            {
                var input = AsyncConsole.ReadLine(stoppingToken)?.Trim();
                if (string.IsNullOrEmpty(input)) continue;

                if (!Uri.TryCreate(input, UriKind.Absolute, out var url))
                {
                    logger.LogWarning("请输入网址");
                    continue;
                }

                await messageOrchestrator.SendAsnc<UrlMessage, Uri>(url, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "无法处理输入");
            }
        }
    }
}