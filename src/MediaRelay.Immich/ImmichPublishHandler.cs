using Apigen.Immich.Client;
using Apigen.Immich.Models;
using MediaRelay.Publish;
using MediaRelay.Storage;
using Microsoft.Extensions.Logging;

namespace MediaRelay.Immich;


internal sealed class ImmichPublishHandler(
    ImmichApiClient apiClient,
    ILogger<ImmichPublishHandler> logger
    ) : IPublishExecutor
{
    public async ValueTask ExecuteAsync(PublishContent content, CancellationToken cancellationToken = default)
    {
        List<Guid> mediaIds = [];

        foreach (var resource in content.Resources.ToArray())
        {
            using var _ = logger.BeginScope("Resource", resource);

            try
            {
                // 媒体上传
                var meidaId = await UploadMediaAsync(content, resource, cancellationToken);

                // 修改描述
                var description = GetDescriptionString(content);
                var mediaUpdateResult = await apiClient.Assets
                    .UpdateAsync(meidaId.ToString(), new() { Description = description }, cancellationToken);

                // 标签
                await TagAssetsAsync(meidaId, content.Tags, cancellationToken);

                // 加入集合
                mediaIds.Add(meidaId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "媒体上传失败");
            }
        }

        using var __ = logger.BeginScope("MediaIds", $"[{string.Join(',', mediaIds)}]");
        try
        {
            await StackMediasAsync(mediaIds, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "媒体堆叠失败");
        }
    }


    // 上传媒体
    private async Task<Guid> UploadMediaAsync(PublishContent content, StorageInfo storageInfo, CancellationToken cancellationToken)
    {
        logger.LogInformation("开始上传媒体");

        var media = await GetMediaCreateDtoAsync(content, storageInfo, cancellationToken);
        var mediaUploadResult = await apiClient.Assets.UploadAssetAsync(media, cancellationToken: cancellationToken);

        ArgumentException.ThrowIfNullOrWhiteSpace(mediaUploadResult.Id);
        var id = Guid.Parse(mediaUploadResult.Id);

        using var _ = logger.BeginScope("MediaId", id);
        logger.LogInformation("媒体上传完成");

        return id;
    }

    // 堆叠媒体
    private async Task StackMediasAsync(List<Guid> assetsIds, CancellationToken cancellationToken)
    {
        if (assetsIds.Count <= 1) return;

        logger.LogInformation("开始堆叠媒体");
        await apiClient.Stacks.CreateAsync(new() { AssetIds = assetsIds }, cancellationToken);
        logger.LogInformation("媒体已堆叠");
    }

    // 打标签
    private async Task TagAssetsAsync(Guid assetsId, IEnumerable<string?> tags, CancellationToken cancellationToken)
    {
        var upsertResults = await apiClient.Tags
            .UpsertTagsAsync(new() { Tags = [.. tags] }, cancellationToken);

        foreach (var result in upsertResults)
        {
            await apiClient.Tags
                .TagAssetsAsync(result.Id!, new() { Ids = [assetsId] }, cancellationToken);

            using var _ = logger.BeginScope("TagId", result.Id ?? string.Empty);
            logger.LogWarning("已为标签添加媒体");
        }
    }


    // 获取描述文本
    private static string GetDescriptionString(PublishContent content)
    {
        return $"{content.Title}{Environment.NewLine}{content.Description}{Environment.NewLine}{content.SourceUrl}";
    }

    // 获取媒体创建Dto
    private static async Task<AssetMediaCreateDto> GetMediaCreateDtoAsync(PublishContent publishContent, StorageInfo info, CancellationToken cancellationToken)
    {
        var fileFullPath = info.Uri.AbsolutePath;
        var bytes = await File.ReadAllBytesAsync(fileFullPath, cancellationToken);
        var fileInfo = new FileInfo(fileFullPath);

        return new()
        {
            DeviceAssetId = $"{info.HashAlgorithm}_{info.Hash}",
            AssetData = bytes,
            DeviceId = "MediaRelay",

            Filename = Path.GetFileName(fileFullPath),
            FileCreatedAt = publishContent.UploadAt.UtcDateTime,
            FileModifiedAt = fileInfo.LastWriteTime
        };
    }
}