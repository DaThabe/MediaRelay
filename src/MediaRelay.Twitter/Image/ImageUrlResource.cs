using MediaRelay.Http;
using MediaRelay.Resource;
using MediaRelay.Storage.Media;

namespace MediaRelay.Twitter.Image;


internal sealed partial class ImageUrlResource : IUrlResource
{
    private readonly IHttpClient _httpClient;

    public ResourceId Id { get; init; }
    public required ImageUrl Url { get; init; }
    public MediaType Type => Url.MediaType;

    Uri IUrlResource.Url => Url.Uri;

    private ImageUrlResource(IHttpClient httpClient) => _httpClient = httpClient;


    public ValueTask<Stream> GetStreamAsync(CancellationToken cancellationToken = default)
    {
        var task = _httpClient.GetStreamAsync(Url.ToString(), cancellationToken);
        return new ValueTask<Stream>(task);
    }
}

internal sealed partial class ImageUrlResource : IResource
{
    public sealed class Factory(IHttpClient httpClient)
    {
        public ImageUrlResource Create(ImageUrl url)
        {
            return new ImageUrlResource(httpClient)
            {
                Id = ResourceId.CreateImageId(url.MediaId, url.Size),
                Url = url
            };
        }
    }
}