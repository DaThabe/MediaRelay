using MediaRelay.Resource;
using MediaRelay.Storage.Hash;
using System.Text;

namespace MediaRelay.Storage;


internal sealed class FileNameFactory(IHasher hasher) : IFileNameFactory
{
    public StorageFileName Create(IResource resource)
    {
        var idString = resource.Id.ToString();
        var bytes = Encoding.UTF8.GetBytes(idString);
        var hashData = hasher.Hash(bytes);

        return StorageFileName.Create(hashData.HexString);
    }
}