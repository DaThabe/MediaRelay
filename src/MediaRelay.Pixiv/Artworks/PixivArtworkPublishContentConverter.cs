using MediaRelay.Content;
using MediaRelay.Publish;
using MediaRelay.Resources;

namespace MediaRelay.Pixiv.Artworks;


internal sealed class PixivArtworkPublishContentConverter(
    IResourceStorage resourceStorage) : IPublishContentConverter
{
    public bool CanConvert(IContent content) => content is PixivArtworkContent;
    public async ValueTask<PublishContent> ConvertAsync(IContent content, CancellationToken cancellationToken = default)
    {
        if (content is not PixivArtworkContent pixivContent)
            throw new ArgumentException("Content must be of type PixivContent", nameof(content));

        // 储存所有资源
        var resourceUris = await resourceStorage
            .StoreAllAsync(pixivContent.MediaResources, cancellationToken);

        return new PublishContent()
        {
            ContentId = pixivContent.Id,
            SourceUri = pixivContent.SourceUri,

            Author = pixivContent.AuthorName,
            AuthorUrl = pixivContent.AuthorUrl,

            Title = pixivContent.Title,
            Description = pixivContent.Description,
            UploadAt = pixivContent.UploadAt,

            Tags = pixivContent.Tags,
            Resources = resourceUris.Values.ToHashSet()
        };
    }
}