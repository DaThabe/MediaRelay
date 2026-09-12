using MediaRelay.Resource;
using Microsoft.Extensions.Logging;
using System.Collections.Frozen;

namespace MediaRelay.Storage;


internal sealed class ResourceStorage(
    IStorage storage,
    ILogger<ResourceStorage> logger) : IResourceStorage
{
    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="ArgumentOutOfRangeException"/>
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


        var index = 0;
        var successed = 0;
        var uris = new Dictionary<ResourceId, StorageInfo>();

        using var _ = logger.BeginScope("ResourceCount", resourcesArray.Length);
        logger.LogInformation("开始储存资源");

        foreach (var resource in resourcesArray)
        {
            using var __ = logger.Scope("Index", index++)
                .Add("Resource", resource)
                .Begin();

            try
            {
                await using var stream = await resource
                .GetStreamAsync(cancellationToken);


                logger.LogInformation("正在储存资源");

                var info = await storage
                    .StoreAsync(stream, resource.Type, cancellationToken);

                logger.LogInformation("资源储已储存");
                uris[resource.Id] = info;
                successed++;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "资源储失败");
            }
        }

        using var ___ = logger.BeginScope("Successed", successed);
        logger.LogInformation("资源储存完毕");

        return uris;
    }
}
