using MediaRelay.Browser;
using MediaRelay.Source;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace MediaRelay.Content.Extract;

/// <summary>
/// 自定义网址来源的网址内容提取器
/// </summary>
/// <typeparam name="TUrlSource">网址来源类型</typeparam>
public abstract class UrlContentExtractor<TUrlSource>(IBrowserService browserService) :
    UrlContentExtractor<TUrlSource, UrlContentExtractorSnapshot, UrlContentBuilder, UrlContent>(browserService)
    where TUrlSource : IUrlSource
{
    protected override UrlContentBuilder CreateContentBuilder(TUrlSource source)
    {
        return new UrlContentBuilder(ContentId.Create(source.Id.ToString()), source);
    }

    protected override JsonTypeInfo<UrlContentExtractorSnapshot> GetScriptResultJsonTypeInfo()
    {
        return UrlContentJsonSerializerContext.Default.UrlContentExtractorSnapshot;
    }
}



[JsonSourceGenerationOptions(
    // 忽略大小写，允许驼峰和帕斯卡命名
    PropertyNameCaseInsensitive = true,
    // 格式化输出
    WriteIndented = true
)]
[JsonSerializable(typeof(UrlContentExtractorSnapshot))]
internal partial class UrlContentJsonSerializerContext : JsonSerializerContext;