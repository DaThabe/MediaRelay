using MediaRelay.Source;
using Microsoft.Extensions.Logging;

namespace MediaRelay.Content;


internal sealed class ContentExtractorSelector(
        IEnumerable<IContentExtractor> extractors,
        ILogger<ContentExtractorSelector> logger
    ) : IContentExtractorSelector
{
    private readonly IContentExtractor[] _extractors = [.. extractors];

    public IContentExtractor Select(ISource source)
    {
        foreach (var extractor in _extractors)
        {
            if (extractor.CanExtract(source))
            {
                using var _ = logger.Scope()
                   .Add("ContentExtractorName", extractor.GetType().Name)
                   .Begin();
                logger.LogDebug("已经选内容提取器");

                return extractor;
            }
        }

        throw new NotSupportedException($"无法提取该输入: {source.GetType().Name}");
    }
}