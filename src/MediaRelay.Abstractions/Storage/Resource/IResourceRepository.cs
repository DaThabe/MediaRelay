using MediaRelay.Resource;
using MediaRelay.Storage.Media;

namespace MediaRelay.Storage.Resource;


public interface IResourceRepository
{
    ValueTask<IReadOnlyDictionary<ResourceId, MediaStorageInfo>> AddRangeAsync(
        IEnumerable<IResource> resources,
        CancellationToken cancellationToken = default);
}