using MediaRelay.Input;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MediaRelay.Console.Input;


internal sealed class InputListenBackgroundService(
    IMediaRelay mediaRelay,
    IInputParserSelector sourceParserSelector,
    ILogger<InputListenBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                System.Console.Write(">>> ");
                var rawInput = System.Console.ReadLine();
                if (string.IsNullOrWhiteSpace(rawInput)) continue;

                _ = Task.Run(async () => await HandleInputAsync(rawInput, stoppingToken), stoppingToken);
            }
            catch (Exception ex)
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine($"发生错误: {ex.Message}");
                System.Console.ResetColor();
            }
        }
    }

    private async ValueTask HandleInputAsync(string rawInput, CancellationToken cancellationToken)
    {
        var input = sourceParserSelector
                    .Select(rawInput)
                    .Parse(rawInput);

        await mediaRelay.HandleAsync(input, cancellationToken);
    }
}
