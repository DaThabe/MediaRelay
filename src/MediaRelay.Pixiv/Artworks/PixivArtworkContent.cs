using MediaRelay.Content;
using MediaRelay.Resources;

namespace MediaRelay.Pixiv.Artworks;

internal sealed partial record class PixivArtworkContent : IWebContent
{
    public required ContentId Id { get; init; }
    public required Uri SourceUri { get; init; }
    public required IReadOnlySet<IResource> MediaResources { get; init; }

    public required long ArtworkId { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string AuthorName { get; init; }
    public required string AuthorUrl { get; init; }
    public required DateTimeOffset UploadAt { get; init; }
    public required IReadOnlySet<string> Tags { get; init; }


    private PixivArtworkContent() { }
    public static Builder BuilderFromArtworkId(Uri sourceUri, long artworkId) => new(sourceUri, artworkId);


    public override string ToString() => Id.ToString();
}


internal sealed partial record class PixivArtworkContent
{
    public class Builder(Uri sourceUri, long artworkId)
    {
        private readonly ContentId _id = ContentId.Create($"Pixiv_Artworks_{artworkId}");
        private readonly HashSet<IResource> _resources = [];
        private string _title = string.Empty;
        private string? _description = string.Empty;
        private string _authorName = string.Empty;
        private string _authorUrl = string.Empty;
        private DateTimeOffset _uploadTime = DateTimeOffset.MinValue;
        private HashSet<string> _tags = [];


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


        public PixivArtworkContent Build()
        {
            return new()
            {
                Id = _id,

                SourceUri = sourceUri,
                ArtworkId = artworkId,
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
