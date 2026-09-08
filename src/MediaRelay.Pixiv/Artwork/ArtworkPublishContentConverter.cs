using MediaRelay.Content;
using MediaRelay.Storage;

namespace MediaRelay.Pixiv.Artwork;


internal sealed class ArtworkPublishContentConverter(
    IResourceStorage resourceStorage) : IContentConverter
{
    public bool CanConvert(IContent content) => content is ArtworkContent;
    public async ValueTask<RelayContent> ConvertAsync(IContent content, CancellationToken cancellationToken = default)
    {
        if (content is not ArtworkContent pixivContent)
            throw new NotSupportedException($"不是有效的Pixiv作品内容: {content.Id}");

        // 储存所有资源
        var resourceUris = await resourceStorage
            .StoreAllAsync(pixivContent.MediaResources, cancellationToken);

        return new RelayContent()
        {
            ContentId = pixivContent.Id,
            SourceUrl = pixivContent.Source.Url,

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