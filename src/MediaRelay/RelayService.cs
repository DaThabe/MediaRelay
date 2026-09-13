using MediaRelay.Payload;
using Microsoft.Extensions.Logging;

namespace MediaRelay;


internal sealed class RelayService(
    IEnumerable<IPayloadHandler> relayHandlers,
    ILogger<RelayService> logger
    ) : IPayloadRelayService
{
    private readonly IPayloadHandler[] _relayHandlers = [.. relayHandlers];

    public async ValueTask RelayAsync(IPayload payload, CancellationToken cancellationToken = default)
    {
        if (_relayHandlers.Length == 0) return;

        var tasks = new List<Task>();

        foreach (var handler in _relayHandlers)
        {
            if (!handler.CanRelay(payload)) continue;
            using var _ = logger.BeginScope("Handler", handler.GetType().Name);

            tasks.Add(handler.RelayAsync(payload, cancellationToken).AsTask());
        }

        await Task.WhenAll(tasks);
    }
}
