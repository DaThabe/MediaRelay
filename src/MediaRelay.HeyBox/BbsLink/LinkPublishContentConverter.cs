using MediaRelay.Content;
using MediaRelay.Storage;

namespace MediaRelay.HeyBox.BbsLink;


internal sealed class LinkPublishContentConverter(
    IResourceStorage resourceStorage) : IRelayContentCreator
{
    public bool CanCreate(IContent content) => content is LinkContent;
    public async ValueTask<RelayPayload> CreateAsync(IContent content, CancellationToken cancellationToken = default)
    {
        if (content is not LinkContent artwork)
            throw new NotSupportedException($"不是有效的 小黑和论坛帖子 内容: {content.Id}");

        // 储存所有资源
        var resourceUris = await resourceStorage
            .StoreAllAsync(artwork.Resources, cancellationToken);

        return new()
        {
            SourceId = artwork.Source.Id,
            ContentId = artwork.Id,

            Author = artwork.AuthorName,
            SourceUrl = artwork.Source.Url,
            AuthorUrl = artwork.AuthorUrl,

            Title = artwork.Title,
            Description = artwork.Content,
            UploadAt = artwork.UploadAt,

            Tags = artwork.Tags,
            Resources = resourceUris.Values.ToHashSet()
        };
    }
}