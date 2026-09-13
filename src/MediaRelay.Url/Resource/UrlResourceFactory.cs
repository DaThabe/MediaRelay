using MediaRelay.Http;
using MediaRelay.Storage;

namespace MediaRelay.Resource;


internal sealed class UrlResourceFactory(IHttpClient httpClient) : IUrlResourceFactory
{
    public IUrlResource Create(ResourceId resourceId, Uri url, MediaType type)
    {
        return new HttpResource(httpClient)
        {
            Id = resourceId,
            Url = url,
            Type = type
        };
    }
}


file sealed class HttpResource(IHttpClient httpClient) : UrlResource
{
    public override ValueTask<Stream> GetStreamAsync(CancellationToken cancellationToken = default)
    {
        var task = httpClient.GetStreamAsync(Url.ToString(), cancellationToken);
        return new ValueTask<Stream>(task);
    }
}