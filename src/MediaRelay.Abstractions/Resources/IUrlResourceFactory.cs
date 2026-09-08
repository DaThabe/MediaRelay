using MediaRelay.Storage;

namespace MediaRelay.Resources;

public interface IUrlResourceFactory
{
    IResource Create(ResourceId resourceId, Uri url, MediaType type);
}