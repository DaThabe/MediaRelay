using MediaRelay.Resource;
using MediaRelay.Storage.Hash;
using System.Text;

namespace MediaRelay.Storage.Resource;


internal sealed class ResourceFileNameFactory(IHasher hasher) : IResourceFileNameFactory
{
    public StorageFileName Create(IResource resource)
    {
        var idString = resource.Id.ToString();
        var bytes = Encoding.UTF8.GetBytes(idString);
        var hashData = hasher.Hash(bytes);

        return StorageFileName.Create(hashData.HexString);
    }
}