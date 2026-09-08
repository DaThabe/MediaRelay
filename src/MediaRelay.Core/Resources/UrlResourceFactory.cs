using MediaRelay.Http;
using MediaRelay.Storage;

namespace MediaRelay.Resources;


internal sealed class UrlResourceFactory(IHttpClient httpClient) : IUrlResourceFactory
{
    public IResource Create(ResourceId resourceId, Uri url, MediaType type)
    {
        return new UrlResource(httpClient) { Id = resourceId, Url = url, Type = type };
    }
}

file sealed class UrlResource(IHttpClient httpClient) : IResource
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
