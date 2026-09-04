using MediaRelay.Resources;

namespace MediaRelay.Storage;


internal sealed class ResourceStorage(IStorage storage) : IResourceStorage
{
    public async ValueTask<IReadOnlyDictionary<ResourceId, StorageResource>> StoreAllAsync(
        IEnumerable<IResource> resources,
        CancellationToken cancellationToken = default)
    {
        var uris = new Dictionary<ResourceId, StorageResource>();

        foreach (var resource in resources)
        {
            await using var stream = await resource
                .GetStreamAsync(cancellationToken);

            var info = await storage
                .StoreAsync(stream, resource.Extensions, cancellationToken);

            uris[resource.Id] = new StorageResource()
            {
                Hash = info.Hash,
                HashAlgorithm = info.HashAlgorithm,
                Size = info.Size,
                Uri = info.Uri,
                Extensions = info.Extensions
            };
        }

        return uris;
    }
}
