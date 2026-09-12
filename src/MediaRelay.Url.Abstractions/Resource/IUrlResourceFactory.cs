using MediaRelay.Storage;

namespace MediaRelay.Resource;


public interface IUrlResourceFactory
{
    IUrlResource Create(ResourceId resourceId, Uri url, MediaType type);
}