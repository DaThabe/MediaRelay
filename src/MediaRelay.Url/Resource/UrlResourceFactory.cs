using MediaRelay.Http;
using MediaRelay.Resources;
using MediaRelay.Storage;

namespace MediaRelay.Url.Resource;


internal sealed class UrlResourceFactory(IHttpClient httpClient) : IUrlResourceFactory
{
    public IUrlResource Create(ResourceId resourceId, Uri url, MediaType type)
    {
        return new UrlResource(httpClient) { Id = resourceId, Url = url, Type = type };
    }
}