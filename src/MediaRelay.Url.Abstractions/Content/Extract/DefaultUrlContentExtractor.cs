using MediaRelay.Browser;
using MediaRelay.Content.Builder;
using MediaRelay.Content.Snapshot;
using MediaRelay.Metadata;
using MediaRelay.Source;
using System.Text.Json.Serialization.Metadata;

namespace MediaRelay.Content.Extract;


public abstract class DefaultUrlContentExtractor(IBrowserService browserService) :
    UrlContentExtractor<DefaultUrlSource, DefaultUrlSnapshot, DefaultUrlContentBuilder, DefaultUrlContent, DefaultUrlMetadataBuilder, DefaultUrlMetadata>(browserService)
{
    protected override DefaultUrlContentBuilder CreateContentBuilder(DefaultUrlSource source)
    {
        return new DefaultUrlContentBuilder(ContentId.Create(source.Id.ToString()), source);
    }

    protected override JsonTypeInfo<DefaultUrlSnapshot> GetSnapshotJsonTypeInfo()
    {
        return DefaultUrlSnapshotJsonSerializerContext.Default.DefaultUrlSnapshot;
    }
}