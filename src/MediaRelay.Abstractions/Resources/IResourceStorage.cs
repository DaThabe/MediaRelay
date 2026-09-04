namespace MediaRelay.Resources;


public interface IResourceStorage
{
    ValueTask<IReadOnlyDictionary<ResourceId, StorageResource>> StoreAllAsync(
        IEnumerable<IResource> resources,
        CancellationToken cancellationToken = default);
}