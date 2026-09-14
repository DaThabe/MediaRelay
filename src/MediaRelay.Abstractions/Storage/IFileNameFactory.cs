using MediaRelay.Resource;

namespace MediaRelay.Storage;


public interface IFileNameFactory
{
    StorageFileName Create(IResource resource);
}