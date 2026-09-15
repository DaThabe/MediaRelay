using MediaRelay.Storage.Media;

namespace MediaRelay.Resource;


public interface IUrlResourceFactory
{
    IUrlResource Create(ResourceId resourceId, Uri url, MediaType type);
}