using MediaRelay.Browser;
using MediaRelay.Source;
using System.Text.Json.Serialization.Metadata;

namespace MediaRelay.Content.Extract;

/// <summary>
/// 默认网址内容提取器
/// </summary>
public abstract class UrlContentExtractor(IBrowserService browserService) :
    UrlContentExtractor<UrlSource, UrlContentExtractorSnapshot, UrlContentBuilder, UrlContent>(browserService)
{
    protected override UrlContentBuilder CreateContentBuilder(UrlSource source)
    {
        return new UrlContentBuilder(ContentId.Create(source.Id.ToString()), source);
    }

    protected override JsonTypeInfo<UrlContentExtractorSnapshot> GetScriptResultJsonTypeInfo()
    {
        return UrlContentJsonSerializerContext.Default.UrlContentExtractorSnapshot;
    }
}
