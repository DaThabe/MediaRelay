using MediaRelay.Payload;

namespace MediaRelay;


public interface IPayloadRelayService
{
    ValueTask RelayAsync(IPayload payload, CancellationToken cancellationToken = default);
}