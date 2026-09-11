using MediaRelay.Content;

namespace MediaRelay.Pixiv.Artwork;


internal sealed partial record class ArtworkContent : UrlContent
{
    private ArtworkContent() { }
    public static Builder BuilderFromSource(ArtworkSource source) => new(source);
}


internal sealed partial record class ArtworkContent
{
    public sealed class Builder(ArtworkSource source) : UrlContentBuilder<Builder, ArtworkContent>(ContentId.Create(source.Id.ToString()))
    {
        protected override Builder This() => this;

        protected override ArtworkContent Build(Snapshot snapshot)
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