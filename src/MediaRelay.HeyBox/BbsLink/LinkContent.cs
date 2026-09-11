using MediaRelay.Content;

namespace MediaRelay.HeyBox.BbsLink;


internal sealed partial record class LinkContent : UrlContent
{
    private LinkContent() { }
    public static Builder BuilderFromSource(LinkSource source) => new(source);
}


internal sealed partial record class LinkContent
{
    public sealed class Builder(LinkSource source) : UrlContentBuilder<Builder, LinkContent>(ContentId.Create(source.Id.ToString()))
    {
        protected override Builder This() => this;

        protected override LinkContent Build(Snapshot snapshot)
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
