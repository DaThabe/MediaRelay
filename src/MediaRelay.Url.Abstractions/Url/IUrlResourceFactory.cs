using MediaRelay.Resources;
using MediaRelay.Storage;

namespace MediaRelay.Url;


public interface IUrlResourceFactory
{
    IUrlResource Create(ResourceId resourceId, Uri url, MediaType type);
}