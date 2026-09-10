using MediaRelay.Http;
using MediaRelay.Resources;
using MediaRelay.Storage;

namespace MediaRelay.Url.Resource;


internal sealed class UrlResource(IHttpClient httpClient) : IUrlResource
{
    public required ResourceId Id { get; init; }
    public required Uri Url { get; init; }
    public required MediaType Type { get; init; }


    public ValueTask<Stream> GetStreamAsync(CancellationToken cancellationToken = default)
    {
        var task = httpClient.GetStreamAsync(Url.ToString(), cancellationToken);
        return new ValueTask<Stream>(task);
    }
}
