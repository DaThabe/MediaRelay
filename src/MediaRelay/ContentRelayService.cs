using MediaRelay.Content;
using MediaRelay.Payload;

namespace MediaRelay;

internal sealed class ContentRelayService(
        IPayloadFactory relayContentFactory,
        IPayloadRelayService relayOrchestrator
    ) : IContentRelayService
{
    public async ValueTask RelayAsync(IContent content, CancellationToken cancellationToken = default)
    {
        var relayContent = await relayContentFactory
            .CreateAsync(content, cancellationToken);

        await relayOrchestrator
            .RelayAsync(relayContent, cancellationToken);
    }
}