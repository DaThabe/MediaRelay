using MediaRelay.Http;
using MediaRelay.Resource;
using MediaRelay.Storage;

namespace MediaRelay.HeyBox.Image;


internal sealed partial class OriginalImageUrlResource : IResource
{
    private readonly IHttpClient _httpClient;

    public required ResourceId Id { get; init; }
    public required OriginalImageUrl Url { get; init; }
    public MediaType Type => Url.MediaType;

    private OriginalImageUrlResource(IHttpClient downloader) => _httpClient = downloader;


    public ValueTask<Stream> GetStreamAsync(CancellationToken cancellationToken = default)
    {
        var task = _httpClient.GetStreamAsync(Url.ToString(), cancellationToken);
        return new ValueTask<Stream>(task);
    }
}

internal sealed partial class OriginalImageUrlResource
{
    public class Factory(IHttpClient httpClient)
    {
        public OriginalImageUrlResource Create(OriginalImageUrl url)
        {
            return new(httpClient)
            {
                Id = ResourceId.FromMediaHash(url.Hash),
                Url = url
            };
        }
    }
}