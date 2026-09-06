using MediaRelay.Resources;

namespace MediaRelay.Pixiv.Image;


internal sealed partial class OriginalImageUrlResource : IResource
{
    private readonly IPixivDownloader _downloader;

    public required ResourceId Id { get; init; }
    public required OriginalImageUrl Url { get; init; }
    string IResource.Extensions => Url.Format.ToString();


    private OriginalImageUrlResource(IPixivDownloader downloader) => _downloader = downloader;


    public ValueTask<Stream> GetStreamAsync(CancellationToken cancellationToken = default)
    {
        return _downloader.DownloadAsync(Url.ToString(), cancellationToken);
    }
}

internal sealed partial class OriginalImageUrlResource
{
    public class Factory(IPixivDownloader downloader)
    {
        public OriginalImageUrlResource Create(OriginalImageUrl url)
        {
            return new(downloader)
            {
                Id = ResourceId.FromPixivArtworkId(url.ArtworkId),
                Url = url
            };
        }
    }
}