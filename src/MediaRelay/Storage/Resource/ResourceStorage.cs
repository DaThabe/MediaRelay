using MediaRelay.Resource;
using MediaRelay.Storage.Media;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Frozen;

namespace MediaRelay.Storage.Resource;


internal sealed class ResourceStorage(
    IMediaRepository storage,
    IResourceFileNameFactory fileNameCreator,
    ILogger<ResourceStorage> logger) : IResourceRepository
{
    public async ValueTask<IReadOnlyDictionary<ResourceId, MediaStorageInfo>> AddRangeAsync(
        IEnumerable<IResource> resources,
        CancellationToken cancellationToken = default)
    {
        var resourcesArray = resources?.ToArray() ?? [];
        if (resourcesArray.Length == 0)
        {
            LogZeroMediaResourceStored();
            return FrozenDictionary<ResourceId, MediaStorageInfo>.Empty;
        }

        var resourceTotalCount = resourcesArray.Length;
        var resourceStorageSequence = 0;
        var storageInfos = new ConcurrentDictionary<ResourceId, MediaStorageInfo>();
        var parallelOptions = new ParallelOptions()
        {
            MaxDegreeOfParallelism = 6,
            CancellationToken = cancellationToken
        };

        await Parallel.ForEachAsync(resourcesArray, parallelOptions, async (resource, ct) =>
        {
            var currentSequence = Interlocked.Increment(ref resourceStorageSequence);

            storageInfos[resource.Id] = await StoreMediaResourceAsync(resource, ct);
            LogMediaResourceStored(resource.Id, currentSequence, resourceTotalCount);
        });

        LogAllMediaResourceStored(resourceTotalCount);
        return storageInfos.AsReadOnly();
    }


    private async ValueTask<MediaStorageInfo> StoreMediaResourceAsync(IResource resource, CancellationToken cancellationToken)
    {
        await using var stream = await resource
                     .GetStreamAsync(cancellationToken);

        var fileName = fileNameCreator
            .Create(resource);

        return await storage
            .AddAsync(stream, resource.Type, fileName, cancellationToken);
    }



    private void LogMediaResourceStored(ResourceId resourceId, int sequence, int total)
    {
        if (!logger.IsEnabled(LogLevel.Information)) return;

        logger.LogInformation("媒体资源已储存 ResourceId={ResourceId}, Sequence={Sequence}, Total={Total}",
            resourceId, sequence, total);
    }

    private void LogZeroMediaResourceStored()
    {
        if (!logger.IsEnabled(LogLevel.Warning)) return;

        logger.LogWarning("资源列表为空, 未储存任何资源");
    }

    private void LogAllMediaResourceStored(int total)
    {
        if (!logger.IsEnabled(LogLevel.Information)) return;

        logger.LogInformation("媒体资源已储存, Total={Total}", total);
    }
}