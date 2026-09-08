using MediaRelay.Content;
using MediaRelay.Storage;

namespace MediaRelay.Twitter.Tweet;


internal sealed class TweetPublishContentConverter(
    IResourceStorage resourceStorage) : IRelayContentConverter
{
    public bool CanConvert(IContent content) => content is TweetContent;
    public async ValueTask<RelayContent> ConvertAsync(IContent content, CancellationToken cancellationToken = default)
    {
        if (content is not TweetContent tweet)
            throw new NotSupportedException($"不支持的推文内容: {content}");

        // 储存所有资源
        var resourceUris = await resourceStorage
            .StoreAllAsync(tweet.MediaResources, cancellationToken);

        return new RelayContent()
        {
            ContentId = tweet.Id,
            SourceUrl = tweet.Source.Url,

            Author = tweet.AuthorName,
            AuthorUrl = tweet.AuthorUrl,

            Title = tweet.Content,
            UploadAt = tweet.UploadAt,

            Tags = tweet.Tags.ToHashSet(),
            Resources = resourceUris.Values.ToHashSet(),
        };
    }
}