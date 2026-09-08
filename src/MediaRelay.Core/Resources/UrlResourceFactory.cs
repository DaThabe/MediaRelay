using MediaRelay.Http;

namespace MediaRelay.Resources;


internal sealed class UrlResourceFactory(IHttpClient httpClient) : IUrlResourceFactory
{
    public IResource Create(ResourceId resourceId, Uri url, string extensions)
    {
        return new UrlResource(httpClient) { Id = resourceId, Url = url, Extensions = extensions };
    }
}

file sealed class UrlResource(IHttpClient httpClient) : IResource
{
    public required ResourceId Id { get; init; }
    public required Uri Url { get; init; }
    public required string Extensions { get; init; }


    public ValueTask<Stream> GetStreamAsync(CancellationToken cancellationToken = default)
    {
        var task = httpClient.GetStreamAsync(Url.ToString(), cancellationToken);
        return new ValueTask<Stream>(task);
    }
}
