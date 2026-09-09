using Microsoft.Extensions.Logging;

namespace MediaRelay;


internal sealed class Relay(
    IEnumerable<IRelayHandler> relayHandlers,
    ILogger<Relay> logger
    ) : IRelay
{
    private readonly IRelayHandler[] _relayHandlers = [.. relayHandlers];

    public async ValueTask RelayAsync(RelayContent content, CancellationToken cancellationToken = default)
    {
        if (_relayHandlers.Length == 0) return;

        var tasks = new List<Task>();

        foreach (var handler in _relayHandlers)
        {
            if (!handler.CanRelay(content)) continue;
            using var _ = logger.BeginScope("Handler", handler.GetType().Name);

            tasks.Add(handler.RelayAsync(content, cancellationToken).AsTask());
        }

        await Task.WhenAll(tasks);
    }
}
