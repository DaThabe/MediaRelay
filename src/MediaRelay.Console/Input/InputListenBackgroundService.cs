using MediaRelay.Input;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MediaRelay.Console.Input;


internal sealed class InputListenBackgroundService(
        IMediaRelay mediaRelay,
        IInputParserSelector sourceParserSelector,
        ILogger<InputListenBackgroundService> logger
    ) : BackgroundService
{
    private int _requestId;
    private readonly CancellationTokenSource _cts = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var input = System.Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(input)) continue;

            if (input == "exit") break;
            OnInputHistoryAdded(new() { Input = input });
        }
    }


    public override void Dispose()
    {
        _cts.Cancel();
        base.Dispose();
    }


    private async void OnInputHistoryAdded(InputMessage message)
    {
        var requestId = Interlocked.Increment(ref _requestId);
        using var _ = logger.BeginScope("RequestId", requestId);

        try
        {
            var input = sourceParserSelector
                       .Select(message.Input)
                       .Parse(message.Input);

            await mediaRelay.HandleAsync(input, _cts.Token);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "处理失败");
        }
    }
}