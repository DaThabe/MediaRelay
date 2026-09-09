using MediaRelay.Source;
using Microsoft.Extensions.Logging;

namespace MediaRelay.Content;


internal sealed class ContentExtractorFactory(
        IEnumerable<IContentExtractor> extractors,
        ILogger<ContentExtractorFactory> logger
    ) : IContentExtractorFactory
{
    private readonly IContentExtractor[] _extractors = [.. extractors];

    public async ValueTask<IContent> CreateAsync(ISource source, CancellationToken cancellationToken = default)
    {
        foreach (var extractor in _extractors)
        {
            if (!extractor.CanExtract(source)) continue;
            {
                using var _ = logger.BeginScope("Extractor", extractor.GetType().Name);
                logger.LogDebug("开始提取");

                return await extractor.ExtractAsync(source, cancellationToken);
            }
        }

        throw new NotSupportedException($"无法提取该输入: {source.GetType().Name}");
    }
}