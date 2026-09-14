using MediaRelay.Resource;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Security.Cryptography;
using System.Text;

namespace MediaRelay.Storage;


internal sealed class ResourceStorage(
    IStorage storage,
    IFileNameFactory fileNameCreator,
    ILogger<ResourceStorage> logger) : IResourceStorage
{
    public async ValueTask<IReadOnlyDictionary<ResourceId, StorageInfo>> StoreAllAsync(
        IEnumerable<IResource> resources,
        CancellationToken cancellationToken = default)
    {
        var resourcesArray = resources?.ToArray() ?? [];
        if (resourcesArray.Length == 0)
        {
            logger.LogWarning("储存了0个资源");
            return FrozenDictionary<ResourceId, StorageInfo>.Empty;
        }

        using var _ = logger.Scope("Total", resourcesArray.Length)
                .Begin();

        var sequence = 0;
        var uris = new ConcurrentDictionary<ResourceId, StorageInfo>();
        var parallelOptions = new ParallelOptions()
        {
            MaxDegreeOfParallelism = 6,
            CancellationToken = cancellationToken
        };

        await Parallel.ForEachAsync(resourcesArray, parallelOptions, async (resource, ct) =>
        {
            var currentSequence = Interlocked.Increment(ref sequence);

            try
            {
                uris[resource.Id] = await StoreResourceAsync(resource, ct);

                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("资源已储存 ResourceId={ResourceId}, Sequence={Sequence}", resource.Id, currentSequence);
                }
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning("资源下载已取消 ResourceId={ResourceId}, Sequence={Sequence}", resource.Id, currentSequence);
                throw;
            }
        });

        logger.LogWarning("资源下载完成");
        return uris.AsReadOnly();
    }


    private async ValueTask<StorageInfo> StoreResourceAsync(IResource resource, CancellationToken cancellationToken)
    {
        await using var stream = await resource
                     .GetStreamAsync(cancellationToken);

        var fileName = fileNameCreator
            .Create(resource);

        return await storage
            .StoreAsync(stream, resource.Type, fileName, cancellationToken);
    }
}