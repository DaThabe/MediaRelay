using MediaRelay.Extensions;
using MediaRelay.Resources;
using Microsoft.Extensions.Logging;

namespace MediaRelay.Storage;


internal sealed class ResourceStorage(IStorage storage, ILogger<ResourceStorage> logger) : IResourceStorage
{
    public async ValueTask<IReadOnlyDictionary<ResourceId, StorageInfo>> StoreAllAsync(
        IEnumerable<IResource> resources,
        CancellationToken cancellationToken = default)
    {
        var uris = new Dictionary<ResourceId, StorageInfo>();
        var index = 0;
        var resourcesArray = resources.ToArray();

        using var _ = logger.BeginScope("ResourceCount", resourcesArray.Length);
        logger.LogInformation("开始储存资源");

        foreach (var resource in resourcesArray)
        {
            await using var stream = await resource
                .GetStreamAsync(cancellationToken);

            using var __ = logger.BeginScope("Index", index++);
            logger.LogInformation("正在储存资源");

            var info = await storage
                .StoreAsync(stream, resource.Extensions, cancellationToken);

            logger.LogInformation("资源储已储存");
            uris[resource.Id] = info;
        }

        logger.LogInformation("资源储存完毕");

        return uris;
    }
}
