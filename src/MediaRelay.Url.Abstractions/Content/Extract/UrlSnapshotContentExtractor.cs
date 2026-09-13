using MediaRelay.Browser;
using MediaRelay.Content.Builder;
using MediaRelay.Content.Snapshot;
using MediaRelay.Metadata;
using MediaRelay.Source;

namespace MediaRelay.Content.Extract;

[Obsolete("先别用")]
public abstract class UrlSnapshotContentExtractor<TContentExtractSnapshot>(IBrowserService browserService) :
    UrlContentExtractor<DefaultUrlSource, TContentExtractSnapshot, DefaultUrlContentBuilder, DefaultUrlContent, DefaultUrlMetadataBuilder, DefaultUrlMetadata>(browserService)
    where TContentExtractSnapshot : IUrlSnapshot
{
    protected override DefaultUrlContentBuilder CreateContentBuilder(DefaultUrlSource source)
    {
        return new DefaultUrlContentBuilder(ContentId.Create(source.Id.ToString()), source);
    }
}
