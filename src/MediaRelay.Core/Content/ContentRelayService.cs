namespace MediaRelay.Content;

internal sealed class ContentRelayService(
        IContentConverterSelector relayContentConverterSelector,
        IRelayOrchestrator relayOrchestrator
    ) : IContentRelayService
{
    public async ValueTask RelayAsync(IContent content, CancellationToken cancellationToken = default)
    {
        var relayContent = await relayContentConverterSelector
           .Select(content)
           .ConvertAsync(content, cancellationToken);

        await relayOrchestrator.RelayAsync(relayContent, cancellationToken);
    }
}