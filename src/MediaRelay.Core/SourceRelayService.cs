using MediaRelay.Content;
using MediaRelay.Source;

namespace MediaRelay;


internal sealed class SourceRelayService(
        IContentExtractorSelector contentExtractorSelector,
        IContentRelayService contentRelayService
    ) : ISourceRelayService
{
    public async ValueTask RelayAsync(
        ISource source,
        CancellationToken cancellationToken = default)
    {
        // Extract
        var content = await contentExtractorSelector
            .Select(source)
            .ExtractAsync(source, cancellationToken);

        // Publish
        await contentRelayService.RelayAsync(content, cancellationToken);
    }
}