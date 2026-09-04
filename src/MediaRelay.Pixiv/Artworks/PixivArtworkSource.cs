using MediaRelay.Source;

namespace MediaRelay.Pixiv.Artworks;

internal sealed record class PixivArtworkSource(Uri Url, long ArtworkId) : IWebPageSource
{
    public SourceId Id { get; } = SourceId.Create($"Pixiv:{ArtworkId}");
    public Uri Url { get; } = Url;
    public override string ToString() => Id.ToString();
}