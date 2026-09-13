using MediaRelay.Metadata;
using MediaRelay.Source;

namespace MediaRelay.Content.Builder;


public sealed class DefaultUrlContentBuilder(ContentId contentId, IUrlSource urlSource) :
    UrlContentBuilder<DefaultUrlContentBuilder, DefaultUrlContent, DefaultUrlMetadataBuilder, DefaultUrlMetadata>
{
    protected override DefaultUrlMetadataBuilder NewMetadataBuilder() => new(this);
    protected override DefaultUrlContentBuilder This() => this;
    public override DefaultUrlContent Build()
    {
        return new()
        {
            Id = contentId,
            Source = urlSource,
            Resources = Resources,
            Metadata = MetadataBuilder.Build()
        };
    }
}

public sealed class DefaultUrlMetadataBuilder(DefaultUrlContentBuilder contentBuilder) :
    UrlMetadataBuilder<DefaultUrlMetadataBuilder, DefaultUrlMetadata, DefaultUrlContentBuilder, DefaultUrlContent>(contentBuilder)
{
    protected override DefaultUrlMetadataBuilder This() => this;
    public override DefaultUrlMetadata Build()
    {
        return new()
        {
            Title = Title,
            Description = Description,
            AuthorName = AuthorName,
            AuthorUrl = AuthorUrl,
            PublishedAt = PublishedAt,
            Tags = Tags
        };
    }
}