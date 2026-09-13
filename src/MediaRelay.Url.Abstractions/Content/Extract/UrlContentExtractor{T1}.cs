using MediaRelay.Browser;
using MediaRelay.Content.Builder;
using MediaRelay.Metadata;
using MediaRelay.Source;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace MediaRelay.Content.Extract;

/// <summary>
/// 自定义网址来源的网址内容提取器
/// </summary>
/// <typeparam name="TUrlSource">网址来源类型</typeparam>
public abstract class UrlContentExtractor<TUrlSource>(IBrowserService browserService) :
    UrlContentExtractor<TUrlSource, DefaultUrlContentExtractorSnapshot, DefaultUrlContentBuilder, DefaultUrlContent, DefaultUrlMetadataBuilder, DefaultUrlMetadata>(browserService)
    where TUrlSource : IUrlSource
{
    protected override DefaultUrlContentBuilder CreateContentBuilder(TUrlSource source)
    {
        return new DefaultUrlContentBuilder(ContentId.Create(source.Id.ToString()), source);
    }

    protected override JsonTypeInfo<DefaultUrlContentExtractorSnapshot> GetScriptResultJsonTypeInfo()
    {
        return UrlContentJsonSerializerContext.Default.DefaultUrlContentExtractorSnapshot;
    }
}


[JsonSourceGenerationOptions(
    // 忽略大小写，允许驼峰和帕斯卡命名
    PropertyNameCaseInsensitive = true,
    // 格式化输出
    WriteIndented = true
)]
[JsonSerializable(typeof(DefaultUrlContentExtractorSnapshot))]
internal partial class UrlContentJsonSerializerContext : JsonSerializerContext;