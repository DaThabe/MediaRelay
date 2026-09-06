using MediaRelay.Resources;

namespace MediaRelay.Storage;


public interface IResourceStorage
{
    ValueTask<IReadOnlyDictionary<ResourceId, StorageInfo>> StoreAllAsync(
        IEnumerable<IResource> resources,
        CancellationToken cancellationToken = default);
}