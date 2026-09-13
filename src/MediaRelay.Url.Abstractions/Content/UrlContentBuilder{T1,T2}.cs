using MediaRelay.Resource;

namespace MediaRelay.Content;


/// <summary>
/// 自定义构建器
/// </summary>
/// <typeparam name="TBuilder">自定义构建器类型</typeparam>
/// <typeparam name="TUrlContent">网址内容类型</typeparam>
public abstract class UrlContentBuilder<TBuilder, TUrlContent> : IUrlContentBuilder<TBuilder, TUrlContent>
    where TUrlContent : IUrlContent
    where TBuilder : IUrlContentBuilder<TBuilder, TUrlContent>
{
    private readonly HashSet<IResource> _resources = [];

    private string? _title;
    private string? _content;
    private DateTimeOffset? _uploadTime;

    private string? _authorName;
    private Uri? _authorUrl;

    private readonly HashSet<string> _tags = [];



    public TBuilder SetTitle(string? title)
    {
        _title = string.IsNullOrWhiteSpace(title) ? null : title.Trim();
        return This();
    }
    public TBuilder SetContent(string? content)
    {
        _content = string.IsNullOrWhiteSpace(content) ? null : content.Trim();
        return This();
    }
    public TBuilder SetUploadTime(DateTimeOffset? uploadTime)
    {
        _uploadTime = uploadTime;
        return This();
    }

    public TBuilder SetAuthorName(string? name)
    {
        _authorName = string.IsNullOrWhiteSpace(name) ? null : name.Trim();
        return This();
    }
    public TBuilder SetAuthorLink(Uri? url)
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

    public TUrlContent Build()
    {
        var snapshot = new Snapshot()
        {
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



    protected abstract TUrlContent Build(Snapshot snapshot);
    protected abstract TBuilder This();


    protected sealed record class Snapshot
    {
        public required IReadOnlySet<IResource> Resources { get; init; }
        public string? Title { get; init; }
        public string? Content { get; init; }
        public string? AuthorName { get; init; }
        public Uri? AuthorUrl { get; init; }
        public DateTimeOffset? UploadAt { get; init; }
        public IReadOnlySet<string> Tags { get; init; } = new HashSet<string>();
    }
}
