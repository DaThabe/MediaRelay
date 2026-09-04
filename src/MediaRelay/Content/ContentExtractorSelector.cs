using MediaRelay.Content;
using MediaRelay.Source;
using Microsoft.Extensions.Logging;

namespace MediaRelay;

internal sealed partial class ContentExtractorSelector(
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
                LogSelected(extractor.GetType().Name, source);
                return extractor;
            }
        }

        throw new NotSupportedException($"无法提取该输入: {source.GetType().Name}");
    }


    [LoggerMessage(Level = LogLevel.Debug, Message = "已选择 [{ExtractorType}] < 来源 [{Source}]")]
    private partial void LogSelected(string extractorType, ISource source);
}