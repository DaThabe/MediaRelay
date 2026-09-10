using MediaRelay.Content;
using MediaRelay.Resources;
using MediaRelay.Url;

namespace MediaRelay.Pixiv.Artwork;


internal sealed partial record class ArtworkContent : IUrlContent
{
    public required ContentId Id { get; init; }
    public required IUrlSource Source { get; init; }
    public required IReadOnlySet<IResource> MediaResources { get; init; }


    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string AuthorName { get; init; }
    public required string AuthorUrl { get; init; }
    public required DateTimeOffset UploadAt { get; init; }
    public required IReadOnlySet<string> Tags { get; init; }



    private ArtworkContent() { }
    public static Builder BuilderFromSource(ArtworkSource source) => new(source);
}


internal sealed partial record class ArtworkContent
{
    public class Builder(ArtworkSource source)
    {
        private readonly ContentId _id = ContentId.Create(source.Id.ToString());
        private readonly HashSet<IResource> _resources = [];
        private string _title = string.Empty;
        private string? _description = string.Empty;
        private string _authorName = string.Empty;
        private string _authorUrl = string.Empty;
        private DateTimeOffset _uploadTime = DateTimeOffset.MinValue;
        private readonly HashSet<string> _tags = [];


        public Builder AddResources(params IEnumerable<IResource> resources)
        {
            _resources.UnionWith(resources);
            return this;
        }
        public Builder AddResource(IResource resources)
        {
            _resources.Add(resources);
            return this;
        }

        public Builder SetTitle(string title)
        {
            _title = title;
            return this;
        }
        public Builder SetDescription(string description)
        {
            _description = description;
            return this;
        }

        public Builder SetAuthor(string name, string url)
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


        public Builder AddTags(params IEnumerable<string> tags)
        {
            _tags.UnionWith(tags.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()));
            return this;
        }


        public ArtworkContent Build()
        {
            return new()
            {
                Id = _id,
                Source = source,
                MediaResources = _resources,

                AuthorName = _authorName,
                AuthorUrl = _authorUrl,

                Title = _title,
                Description = _description ?? string.Empty,

                UploadAt = _uploadTime,
                Tags = _tags
            };
        }
    }
}
