using MediaRelay.Resources;

namespace MediaRelay.Content;


public interface IUrlContentBuilder<TBuilder, TContent>
    where TBuilder : IUrlContentBuilder<TBuilder, TContent>
    where TContent : IUrlContent
{
    TBuilder SetTitle(string title);
    TBuilder SetContent(string content);
    TBuilder SetUploadTime(DateTimeOffset uploadTime);
    TBuilder SetAuthorName(string name);
    TBuilder SetAuthorLink(Uri url);
    TBuilder AddTags(params IEnumerable<string> tags);
    TBuilder AddResources(params IEnumerable<IResource> resources);

    TContent Build();
}
public static class UrlContentBuilderExtensions
{
    extension<TBuilder, TContent>(IUrlContentBuilder<TBuilder, TContent> builder)
        where TBuilder : IUrlContentBuilder<TBuilder, TContent>
        where TContent : IUrlContent
    {
        public TBuilder SetAuthorLink(string url)
        {
            return builder.SetAuthorLink(new Uri(url));
        }

        public TBuilder SetAuthor(string name, Uri url)
        {
            return builder.SetAuthorName(name).SetAuthorLink(url);
        }
        public TBuilder SetAuthor(string name, string url)
        {
            return builder.SetAuthorName(name).SetAuthorLink(url);
        }
    }
}


public abstract class UrlContentBuilder<TBuilder, TContent>(ContentId contentId) : IUrlContentBuilder<TBuilder, TContent>
    where TContent : IUrlContent
    where TBuilder : IUrlContentBuilder<TBuilder, TContent>
{
    private readonly ContentId _id = contentId;
    private readonly HashSet<IResource> _resources = [];

    private string? _title;
    private string? _content;
    private DateTimeOffset? _uploadTime;

    private string? _authorName;
    private Uri? _authorUrl;

    private readonly HashSet<string> _tags = [];



    public TBuilder SetTitle(string title)
    {
        _title = string.IsNullOrWhiteSpace(title) ? null : title.Trim();
        return This();
    }
    public TBuilder SetContent(string content)
    {
        _content = string.IsNullOrWhiteSpace(content) ? null : content.Trim();
        return This();
    }
    public TBuilder SetUploadTime(DateTimeOffset uploadTime)
    {
        _uploadTime = uploadTime;
        return This();
    }

    public TBuilder SetAuthorName(string name)
    {
        _authorName = string.IsNullOrWhiteSpace(name) ? null : name.Trim();
        return This();
    }
    public TBuilder SetAuthorLink(Uri url)
    {
        _authorUrl = url;
        return This();
    }


    public TBuilder AddTags(params IEnumerable<string> tags)
    {
        _tags.UnionWith(tags.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()));
        return This();
    }
    public TBuilder AddResources(params IEnumerable<IResource> resources)
    {
        _resources.UnionWith(resources);
        return This();
    }

    public TContent Build()
    {
        var snapshot = new Snapshot()
        {
            Id = _id,
            Resources = _resources,
            AuthorName = _authorName,
            AuthorUrl = _authorUrl,

            Title = _title,
            Content = _content,
            UploadAt = _uploadTime,

            Tags = _tags
        };

        return Build(snapshot);
    }



    protected abstract TContent Build(Snapshot snapshot);
    protected abstract TBuilder This();


    protected sealed record class Snapshot
    {
        public required ContentId Id { get; init; }
        public required IReadOnlySet<IResource> Resources { get; init; }
        public string? Title { get; init; }
        public string? Content { get; init; }
        public string? AuthorName { get; init; }
        public Uri? AuthorUrl { get; init; }
        public DateTimeOffset? UploadAt { get; init; }
        public IReadOnlySet<string> Tags { get; init; } = new HashSet<string>();
    }
}