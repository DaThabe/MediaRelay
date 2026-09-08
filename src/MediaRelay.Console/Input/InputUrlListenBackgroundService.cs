using AsyncConsoleReader;
using MediaRelay.Url;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MediaRelay.Console.Input;


internal sealed class InputUrlListenBackgroundService(
        IUrlRelayService urlRelay,
        ILogger<InputUrlListenBackgroundService> logger
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

                await urlRelay.RelayAsync(url, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "无法处理输入");
            }
        }
    }
}