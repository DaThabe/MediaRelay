using MediaRelay.Content;

namespace MediaRelay.Twitter.Tweet;


internal sealed partial record class TweetContent : UrlContent
{
    private TweetContent() { }
    public static Builder BuilderFromSource(TweetSource source) => new(source);
}

internal sealed partial record class TweetContent
{
    public sealed class Builder(TweetSource source) :
        UrlContentBuilder<Builder, TweetContent>(ContentId.Create(source.Id.ToString()))
    {
        protected override Builder This() => this;

        protected override TweetContent Build(Snapshot snapshot)
        {
            return new()
            {
                Id = snapshot.Id,
                Source = source,
                Resources = snapshot.Resources,

                AuthorName = snapshot.AuthorName,
                AuthorUrl = snapshot.AuthorUrl,

                Title = snapshot.Title,
                Content = snapshot.Content,

                UploadAt = snapshot.UploadAt,
                Tags = snapshot.Tags
            };
        }
    }
}
