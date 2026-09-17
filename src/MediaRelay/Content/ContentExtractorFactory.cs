using MediaRelay.Source;

namespace MediaRelay.Content;


internal sealed class ContentExtractorFactory(
        IEnumerable<IContentExtractor> extractors
    ) : IContentExtractorFactory
{
    private readonly IContentExtractor[] _extractors = [.. extractors];

    public async ValueTask<IContent> CreateAsync(ISource source, CancellationToken cancellationToken = default)
    {
        foreach (var extractor in _extractors)
        {
            if (!extractor.CanExtract(source)) continue;
            return await extractor.ExtractAsync(source, cancellationToken);
        }

        throw new NotSupportedException($"无法提取该输入: {source.GetType().Name}");
    }
}