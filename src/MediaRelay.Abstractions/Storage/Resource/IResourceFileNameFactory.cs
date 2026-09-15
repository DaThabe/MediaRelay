using MediaRelay.Resource;

namespace MediaRelay.Storage.Resource;


public interface IResourceFileNameFactory
{
    StorageFileName Create(IResource resource);
}