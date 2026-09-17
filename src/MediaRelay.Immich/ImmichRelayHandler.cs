using Apigen.Immich.Client;
using Apigen.Immich.Models;
using MediaRelay.Payload;
using MediaRelay.Storage.Media;
using Microsoft.Extensions.Logging;

namespace MediaRelay.Immich;


internal sealed class ImmichRelayHandler(
    ImmichApiClient apiClient,
    ILogger<ImmichRelayHandler> logger
    ) : IPayloadHandler
{
    public bool CanHandle(IPayload payload) => true;

    public async ValueTask HandleAsync(IPayload payload, CancellationToken cancellationToken = default)
    {
        List<Guid> mediaIds = [];

        foreach (var resource in payload.Resources.ToArray())
        {
            try
            {
                // 媒体上传
                var meidaId = await UploadMediaAsync(payload, resource, cancellationToken);

                // 修改描述
                var description = GetDescriptionString(payload);
                var mediaUpdateResult = await apiClient.Assets
                    .UpdateAsync(meidaId.ToString(), new() { Description = description }, cancellationToken);

                // 标签
                await TagAssetsAsync(meidaId, payload.Metadata.Tags, cancellationToken);

                // 加入集合
                mediaIds.Add(meidaId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "资源处理失败, ResourceUri={Uri}", resource.Uri);
            }
        }

        try
        {
            await StackMediasAsync(mediaIds, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "媒体堆叠失败, SourceId={SourceId}", payload.Source.Id);
        }
    }


    // 上传媒体
    private async Task<Guid> UploadMediaAsync(IPayload payload, MediaStorageInfo storageInfo, CancellationToken cancellationToken)
    {
        if (logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information))
            logger.LogInformation("开始上传媒体资源, Uri={Uri}", storageInfo.Uri);

        var media = await GetMediaCreateDtoAsync(payload, storageInfo, cancellationToken);
        var mediaUploadResult = await apiClient.Assets.UploadAssetAsync(media, cancellationToken: cancellationToken);

        ArgumentException.ThrowIfNullOrWhiteSpace(mediaUploadResult.Id);
        var id = Guid.Parse(mediaUploadResult.Id);

        if (logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information))
            logger.LogInformation("媒体资源上传完成, Uri={Uri}", storageInfo.Uri);

        return id;
    }

    // 堆叠媒体
    private async Task StackMediasAsync(List<Guid> assetsIds, CancellationToken cancellationToken)
    {
        if (assetsIds.Count <= 1) return;

        try
        {
            logger.LogInformation("开始堆叠媒体");
            await apiClient.Stacks.CreateAsync(new() { AssetIds = assetsIds }, cancellationToken);
            logger.LogInformation("媒体已堆叠");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "媒体队列失败");

            throw;
        }
    }

    // 打标签
    private async Task TagAssetsAsync(Guid assetsId, IReadOnlySet<string> tags, CancellationToken cancellationToken)
    {
        var tagList = tags?.ToList() ?? [];
        if (tagList.Count == 0) return;

        var upsertResults = await apiClient.Tags
            .UpsertTagsAsync(new() { Tags = tagList! }, cancellationToken);

        List<string> addedTags = [];

        foreach (var result in upsertResults)
        {
            var tagName = result.Name;
            try
            {
                await apiClient.Tags
                    .TagAssetsAsync(result.Id!, new() { Ids = [assetsId] }, cancellationToken);
                addedTags.Add(tagName ?? "Empty");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "媒体添加标签失败");
            }
        }

        if (logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information))
        {
            var tagsString = string.Join(',', addedTags);
            logger.LogInformation("已为媒体添加标签, Tags=[{Tags}]", tagsString);
        }
    }


    // 获取描述文本
    private static string GetDescriptionString(IPayload payload)
    {
        if (payload is IUrlPayload urlPayload)
        {
            return $"{urlPayload.Metadata.Title}{Environment.NewLine}{urlPayload.Metadata.Description}{Environment.NewLine}{urlPayload.Source.Url}";
        }
        else
        {
            return payload.Metadata.Title + Environment.NewLine + payload.Metadata.Description;
        }
    }

    // 获取媒体创建Dto
    private static async Task<AssetMediaCreateDto> GetMediaCreateDtoAsync(IPayload publishContent, MediaStorageInfo info, CancellationToken cancellationToken)
    {
        var fileFullPath = info.Uri.AbsolutePath;
        var bytes = await File.ReadAllBytesAsync(fileFullPath, cancellationToken);
        var fileInfo = new FileInfo(fileFullPath);

        return new()
        {
            DeviceAssetId = info.HashInfo.ToString(),
            AssetData = bytes,
            DeviceId = "MediaRelay",

            Filename = Path.GetFileName(fileFullPath),
            FileCreatedAt = publishContent.Metadata.PublishedAt?.UtcDateTime ?? fileInfo.CreationTimeUtc,
            FileModifiedAt = fileInfo.LastWriteTime
        };
    }
}