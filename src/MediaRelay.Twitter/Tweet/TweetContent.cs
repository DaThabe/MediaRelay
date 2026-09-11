using MediaRelay.Content;
using MediaRelay.Resources;
using MediaRelay.Url;

namespace MediaRelay.Twitter.Tweet;


internal sealed partial record class TweetContent : IUrlContent
{
    public required ContentId Id { get; init; }
    public required IUrlSource Source { get; init; }
    public required IReadOnlySet<IResource> Resources { get; init; }


    public required string Content { get; init; }
    public required string AuthorName { get; init; }
    public required Uri AuthorUrl { get; init; }
    public required DateTimeOffset UploadAt { get; init; }
    public required IReadOnlySet<string> Tags { get; init; }


    private TweetContent() { }
    public static Builder BuilderFromSource(TweetSource source) => new(source);
    
    
    string? IUrlContent.Title => null;
    DateTimeOffset? IUrlContent.UploadAt => UploadAt;
}


internal sealed partial record class TweetContent
{
    public class Builder(TweetSource source)
    {
        private readonly ContentId _id = ContentId.Create(source.Id.ToString());
        private readonly HashSet<IResource> _resources = [];
        private string _content = string.Empty;
        private string _authorName = string.Empty;
        private Uri? _authorUrl;
        private DateTimeOffset _uploadTime = DateTimeOffset.MinValue;
        private readonly HashSet<string> _tags = [];


        public Builder AddResources(params IEnumerable<IResource> resources)
        {
            _resources.UnionWith(resources);
            return this;
        }
        public Builder AddTags(params IEnumerable<string> tags)
        {
            _tags.UnionWith(tags.Where(x => !string.IsNullOrWhiteSpace(x)));
            return this;
        }

        public Builder SetContent(string content)
        {
            _content = content;
            return this;
        }

        public Builder SetAuthor(string name, Uri url)
        {
            _authorName = name;
            _authorUrl = url;
            return this;
        }

        public Builder SetUploadTime(DateTimeOffset uploadTime)
        {
            _uploadTime = uploadTime;
            return this;
        }

        public TweetContent Build()
        {
            if (_resources.Count == 0)
                throw new ArgumentException($"推文媒体必须要有1个以上: {_id}");

            return new()
            {
                Id = _id,
                Source = source,
                Resources = _resources,

                AuthorName = _authorName,
                AuthorUrl = _authorUrl,

                Content = _content,
                UploadAt = _uploadTime,
                Tags = _tags
            };
        }
    }
}
