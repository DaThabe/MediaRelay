namespace MediaRelay.Content;

internal sealed class ContentRelayService(
        IRelayContentFactory relayContentFactory,
        IRelayService relayOrchestrator
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