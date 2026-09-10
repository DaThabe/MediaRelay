using MediaRelay.Content;
using MediaRelay.Storage;

namespace MediaRelay.Pixiv.Artwork;


internal sealed class ArtworkPublishContentConverter(
    IResourceStorage resourceStorage) : IRelayContentCreator
{
    public bool CanCreate(IContent content) => content is ArtworkContent;
    public async ValueTask<RelayPayload> CreateAsync(IContent content, CancellationToken cancellationToken = default)
    {
        if (content is not ArtworkContent artwork)
            throw new NotSupportedException($"不是有效的Pixiv作品内容: {content.Id}");

        // 储存所有资源
        var resourceUris = await resourceStorage
            .StoreAllAsync(artwork.MediaResources, cancellationToken);

        return new()
        {
            SourceId = artwork.Source.Id,
            ContentId = artwork.Id,

            Author = artwork.AuthorName,
            SourceUrl = artwork.Source.Url,
            AuthorUrl = artwork.AuthorUrl,

            Title = artwork.Title,
            Description = artwork.Description,
            UploadAt = artwork.UploadAt,

            Tags = artwork.Tags,
            Resources = resourceUris.Values.ToHashSet()
        };
    }
}