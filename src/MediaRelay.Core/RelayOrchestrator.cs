using Microsoft.Extensions.Logging;

namespace MediaRelay;


internal sealed class RelayOrchestrator(
    IEnumerable<IRelayService> relayServices,
    ILogger<RelayOrchestrator> logger
    ) : IRelayOrchestrator
{
    private readonly IRelayService[] _relayServices = [.. relayServices];

    public async ValueTask RelayAsync(RelayContent content, CancellationToken cancellationToken = default)
    {
        if (_relayServices.Length == 0) return;

        var tasks = new List<Task>();

        foreach (var executor in _relayServices)
        {
            var task = executor.RelayAsync(content, cancellationToken).AsTask();
            tasks.Add(task);
        }

        logger.LogInformation("正在转发");

        await Task.WhenAll(tasks);

        logger.LogInformation("转发完成");
    }
}
