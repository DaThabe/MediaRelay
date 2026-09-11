using MediaRelay.Storage;

namespace MediaRelay.Resources;


public interface IUrlResourceFactory
{
    IUrlResource Create(ResourceId resourceId, Uri url, MediaType type);
}