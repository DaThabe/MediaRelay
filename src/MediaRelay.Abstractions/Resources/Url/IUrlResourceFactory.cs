using MediaRelay.Storage;

namespace MediaRelay.Resources.Url;


public interface IUrlResourceFactory
{
    IUrlResource Create(ResourceId resourceId, Uri url, MediaType type);
}