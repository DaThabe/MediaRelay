using MediaRelay.Metadata;
using MediaRelay.Resource;

namespace MediaRelay.Content.Builder;

public abstract class UrlContentBuilder<TUrlContentBuilder, TContent, TMetadataBuilder, TMetadata> : IUrlContentBuilder<TUrlContentBuilder, TContent, TMetadataBuilder, TMetadata>
    where TUrlContentBuilder : IUrlContentBuilder<TUrlContentBuilder, TContent, TMetadataBuilder, TMetadata>
    where TContent : IUrlContent
    where TMetadataBuilder : IUrlMetadataBuilder<TMetadataBuilder, TMetadata, TUrlContentBuilder, TContent>
    where TMetadata : IUrlMetadata
{
    private readonly HashSet<IResource> _resources = [];


    public TMetadataBuilder MetadataBuilder { get; private set; }
    protected IReadOnlySet<IResource> Resources => _resources.AsReadOnly();


    protected UrlContentBuilder()
    {
        MetadataBuilder = NewMetadataBuilder();
    }

    public TUrlContentBuilder AddResources(params IEnumerable<IResource> resources)
    {
        _resources.UnionWith(resources);
        return This();
    }

    protected abstract TUrlContentBuilder This();
    protected abstract TMetadataBuilder NewMetadataBuilder();
    public abstract TContent Build();
}

public abstract class UrlMetadataBuilder<TMetadataBuilder, TMetadata, TUrlContentBuilder, TContent>(TUrlContentBuilder contentBuilder) : IUrlMetadataBuilder<TMetadataBuilder, TMetadata, TUrlContentBuilder, TContent>
    where TMetadataBuilder : IUrlMetadataBuilder<TMetadataBuilder, TMetadata, TUrlContentBuilder, TContent>
    where TMetadata : IUrlMetadata
    where TUrlContentBuilder : IUrlContentBuilder<TUrlContentBuilder, TContent, TMetadataBuilder, TMetadata>
    where TContent : IUrlContent
{
    protected string? Title { get; private set; }
    protected string? Description { get; private set; }
    protected DateTimeOffset? PublishedAt { get; private set; }

    protected string? AuthorName { get; private set; }
    protected Uri? AuthorUrl { get; private set; }

    private readonly HashSet<string> _tags = [];
    public IReadOnlySet<string> Tags => _tags.AsReadOnly();


    public TUrlContentBuilder ContentBuilder => contentBuilder;



    public TMetadataBuilder SetTitle(string? title)
    {
        Title = string.IsNullOrWhiteSpace(title) ? null : title.Trim();
        return This();
    }
    public TMetadataBuilder SetDescription(string? description)
    {
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        return This();
    }

    public TMetadataBuilder SetAuthorName(string? name)
    {
        AuthorName = string.IsNullOrWhiteSpace(name) ? null : name.Trim();
        return This();
    }
    public TMetadataBuilder SetAuthorLink(Uri? url)
    {
        AuthorUrl = url;
        return This();
    }

    public TMetadataBuilder SetPublishedAt(DateTimeOffset? time)
    {
        PublishedAt = time;
        return This();
    }
    public TMetadataBuilder AddTags(params IEnumerable<string> tags)
    {
        _tags.UnionWith(tags);
        return This();
    }


    protected abstract TMetadataBuilder This();
    public abstract TMetadata Build();
}