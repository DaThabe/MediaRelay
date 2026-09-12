using MediaRelay.Http;
using MediaRelay.Resource;
using MediaRelay.Storage;

namespace MediaRelay.Resource;


internal sealed class UrlResourceFactory(IHttpClient httpClient) : IUrlResourceFactory
{
    public IUrlResource Create(ResourceId resourceId, Uri url, MediaType type)
    {
        return new UrlResource(httpClient) { Id = resourceId, Url = url, Type = type };
    }
}