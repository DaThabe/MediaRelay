using MediaRelay.Browser;
using MediaRelay.Content.Builder;
using MediaRelay.Content.Snapshot;
using MediaRelay.Metadata;
using MediaRelay.Source;
using System.Text.Json.Serialization.Metadata;

namespace MediaRelay.Content.Extract;


/// <summary>
/// 自定义网址来源的网址内容提取器
/// </summary>
/// <typeparam name="TUrlSource">网址来源类型</typeparam>
public abstract class UrlSourceContentExtractor<TUrlSource>(IBrowserService browserService) :
    UrlContentExtractor<TUrlSource, DefaultUrlSnapshot, DefaultUrlContentBuilder, DefaultUrlContent, DefaultUrlMetadataBuilder, DefaultUrlMetadata>(browserService)
    where TUrlSource : IUrlSource
{
    protected override DefaultUrlContentBuilder CreateContentBuilder(TUrlSource source)
    {
        return new DefaultUrlContentBuilder(ContentId.Create(source.Id.ToString()), source);
    }

    protected override JsonTypeInfo<DefaultUrlSnapshot> GetSnapshotJsonTypeInfo()
    {
        return DefaultUrlSnapshotJsonSerializerContext.Default.DefaultUrlSnapshot;
    }
}