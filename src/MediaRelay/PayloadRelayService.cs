using MediaRelay.Payload;
using Microsoft.Extensions.Logging;

namespace MediaRelay;


internal sealed class PayloadRelayService(
    IEnumerable<IPayloadHandler> payloadHandlers,
    ILogger<PayloadRelayService> logger
    ) : IPayloadRelayService
{
    private readonly IPayloadHandler[] _payloadHandlers = [.. payloadHandlers];

    public async ValueTask RelayAsync(IPayload payload, CancellationToken cancellationToken = default)
    {
        if (_payloadHandlers.Length == 0) return;

        var tasks = new List<Task>();

        foreach (var handler in _payloadHandlers)
        {
            if (!handler.CanHandle(payload)) continue;
            using var _ = logger.BeginScope("Handler", handler.GetType().Name);

            tasks.Add(handler.HandleAsync(payload, cancellationToken).AsTask());
        }

        await Task.WhenAll(tasks);
    }
}
