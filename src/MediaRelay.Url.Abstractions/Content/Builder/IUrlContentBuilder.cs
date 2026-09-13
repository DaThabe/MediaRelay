using MediaRelay.Metadata;
using MediaRelay.Resource;

namespace MediaRelay.Content.Builder;


public interface IUrlContentBuilder<TUrlContentBuilder, TContent, TMetadataBuilder, TMetadata>
    where TUrlContentBuilder : IUrlContentBuilder<TUrlContentBuilder, TContent, TMetadataBuilder, TMetadata>
    where TContent : IUrlContent
    where TMetadataBuilder : IUrlMetadataBuilder<TMetadataBuilder, TMetadata, TUrlContentBuilder, TContent>
    where TMetadata : IUrlMetadata
{
    TMetadataBuilder MetadataBuilder { get; }
    TUrlContentBuilder AddResources(params IEnumerable<IResource> resources);

    TContent Build();
}

public interface IUrlMetadataBuilder<TMetadataBuilder, TMetadata, TContentBuilder, TContent>
    where TMetadataBuilder : IUrlMetadataBuilder<TMetadataBuilder, TMetadata, TContentBuilder, TContent>
    where TMetadata : IUrlMetadata
    where TContentBuilder : IUrlContentBuilder<TContentBuilder, TContent, TMetadataBuilder, TMetadata>
    where TContent : IUrlContent
{
    TContentBuilder ContentBuilder { get; }

    TMetadataBuilder SetTitle(string? title);
    TMetadataBuilder SetDescription(string? description);

    TMetadataBuilder SetAuthorName(string? name);
    TMetadataBuilder SetAuthorLink(Uri? url);

    TMetadataBuilder SetPublishedAt(DateTimeOffset? time);
    TMetadataBuilder AddTags(params IEnumerable<string> tags);

    TMetadata Build();
}

public static class UrlContentBuilderExtensions
{
    extension<TMetadataBuilder, TMetadata, TContentBuilder, TContent>(IUrlMetadataBuilder<TMetadataBuilder, TMetadata, TContentBuilder, TContent> builder)
        where TMetadataBuilder : IUrlMetadataBuilder<TMetadataBuilder, TMetadata, TContentBuilder, TContent>
        where TMetadata : IUrlMetadata
        where TContentBuilder : IUrlContentBuilder<TContentBuilder, TContent, TMetadataBuilder, TMetadata>
        where TContent : IUrlContent
    {
        public TMetadataBuilder SetAuthorLink(string? url)
        {
            if (string.IsNullOrWhiteSpace(url)) return builder.SetAuthorLink(null);
            return builder.SetAuthorLink(new Uri(url));
        }

        public TMetadataBuilder SetAuthor(string? name, Uri? url)
        {
            return builder.SetAuthorName(name).SetAuthorLink(url);
        }
        public TMetadataBuilder SetAuthor(string? name, string? url)
        {
            return builder.SetAuthorName(name).SetAuthorLink(url);
        }

        public TMetadataBuilder FromMetadata(IUrlMetadata metadata)
        {
            return builder
                .SetAuthor(metadata.AuthorName, metadata.AuthorUrl)
                .SetTitle(metadata.Title)
                .SetDescription(metadata.Description)
                .SetPublishedAt(metadata.PublishedAt)
                .AddTags(metadata.Tags);
        }
    }
}
