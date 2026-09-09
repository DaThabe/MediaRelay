using MediaRelay.Content;

namespace MediaRelay.Source;


internal sealed class SourceRelayService(
        IContentExtractorFactory contentExtractorFactory,
        IContentRelayService contentRelayService
    ) : ISourceRelayService
{
    public async ValueTask RelayAsync(
        ISource source,
        CancellationToken cancellationToken = default)
    {
        var content = await contentExtractorFactory
            .CreateAsync(source, cancellationToken);

        await contentRelayService
            .RelayAsync(content, cancellationToken);
    }
}