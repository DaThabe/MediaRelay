using Apigen.Immich.Client;
using MediaRelay.Publish;
using System.Runtime.CompilerServices;

namespace MediaRelay.Immich;


internal sealed class ImmichPublishHandler(ImmichApiClient apiClient) : IPublishExecutor
{
    public async ValueTask ExecuteAsync(PublishContent content, CancellationToken cancellationToken = default)
    {
        try
        {
            // 上传
            var uploadassetsIds = await UploadAssetsAsync(content, cancellationToken)
                .ToListAsync(cancellationToken: cancellationToken);

            // 堆叠
            await StacksAsync(uploadassetsIds, cancellationToken);

            // 打标签
            var tagAssetsTasks = uploadassetsIds.Select(x => TagAssetsAsync(x, content.Tags, cancellationToken));
            await Task.WhenAll(tagAssetsTasks);
        }
        catch (Exception ex)
        {
            Console.Write(ex);
        }
    }

    private async IAsyncEnumerable<Guid> UploadAssetsAsync(
        PublishContent content,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var resource in content.Resources.ToArray())
        {
            var fileFullPath = resource.Uri.AbsolutePath;
            var bytes = await File.ReadAllBytesAsync(fileFullPath, cancellationToken);

            var fileInfo = new FileInfo(fileFullPath);

            var uploadResult = await apiClient.Assets.UploadAssetAsync(new()
            {
                DeviceAssetId = $"{resource.HashAlgorithm}_{resource.Hash}",
                AssetData = bytes,
                DeviceId = "MediaRelay",

                Filename = Path.GetFileName(resource.Uri.AbsolutePath),
                FileCreatedAt = content.UploadAt.UtcDateTime,
                FileModifiedAt = fileInfo.LastWriteTime

            }, cancellationToken: cancellationToken);

            var describe = $"{content.Title}{Environment.NewLine}{content.Description}{Environment.NewLine}{content.SourceUri}";

            _ = await apiClient.Assets.UpdateAsync(uploadResult.Id!,
                new() { Description = describe }, cancellationToken);

            yield return Guid.Parse(uploadResult.Id!);
        }
    }

    private async Task StacksAsync(List<Guid> assetsIds, CancellationToken cancellationToken)
    {
        if (assetsIds.Count <= 1) return;
        await apiClient.Stacks.CreateAsync(new() { AssetIds = assetsIds }, cancellationToken);
    }

    private async Task TagAssetsAsync(Guid assetsId, IEnumerable<string?> tags, CancellationToken cancellationToken)
    {
        var upsertResults = await apiClient.Tags
            .UpsertTagsAsync(new() { Tags = [.. tags] }, cancellationToken);

        foreach (var result in upsertResults)
        {
            await apiClient.Tags.TagAssetsAsync(result.Id!, new() { Ids = [assetsId] }, cancellationToken);
        }
    }
}