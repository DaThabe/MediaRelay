using MediaRelay.Storage.Hash;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MediaRelay.Storage;


public sealed class StorageInfoRepository(
    IOptions<StorageOptions> options,
    ILogger<StorageInfoRepository> logger
    ) : IStorageInfoRepository
{
    public async ValueTask<StorageInfo?> FindAsync(StorageFileName fileName, MediaType mediaType, CancellationToken cancellationToken)
    {
        var metadataFullPath = GetMetadataFullPath(fileName, mediaType);
        if (!File.Exists(metadataFullPath)) return null;

        try
        {
            var metadataString = await File.ReadAllTextAsync(metadataFullPath, cancellationToken);
            var metadata = JsonSerializer.Deserialize(metadataString, StorageMetadataJsonSerializerContext.Default.StorageMetadata);
            ArgumentNullException.ThrowIfNull(metadata);

            return new StorageInfo()
            {
                HashInfo = HashInfo.Create(metadata.HashAlgorithm, metadata.HashData),
                MediaType = mediaType,
                Size = metadata.Size,
                Uri = new Uri(GetMediaFullPath(fileName, mediaType))
            };
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "无法解析文件元数据");
            return null;
        }
    }

    public async ValueTask SetAsync(StorageFileName fileName, MediaType mediaType, StorageInfo storageInfo, CancellationToken cancellationToken)
    {
        var metadataFullPath = GetMetadataFullPath(fileName, mediaType);

        var metadata = new StorageMetadata()
        {
            HashAlgorithm = storageInfo.HashInfo.Algorithm,
            HashData = storageInfo.HashInfo.Data,
            Size = storageInfo.Size
        };

        var metadataString = JsonSerializer.Serialize(metadata, StorageMetadataJsonSerializerContext.Default.StorageMetadata);
        await File.WriteAllTextAsync(metadataFullPath, metadataString, cancellationToken);

        logger.LogDebug("已设置储存信息");
    }


    private string GetMediaFullPath(StorageFileName fileName, MediaType mediaType)
    {
        // 合并路径
        return Path.Combine(AppContext.BaseDirectory, options.Value.RootPath, $"{fileName}.{mediaType.Extensions}");
    }
    private string GetMetadataFullPath(StorageFileName fileName, MediaType mediaType, string metadataExtension = "json")
    {
        // 合并路径
        return Path.Combine(AppContext.BaseDirectory, options.Value.RootPath, $"{fileName}.{mediaType.Extensions}.{metadataExtension}");
    }
}


internal record class StorageMetadata
{
    public required string HashAlgorithm { get; init; }
    public required byte[] HashData { get; init; }
    public required long Size { get; init; }
}


[JsonSourceGenerationOptions(
    // 忽略大小写，允许驼峰和帕斯卡命名
    PropertyNameCaseInsensitive = true,
    // 格式化输出
    WriteIndented = true,
    // 字符串枚举
    UseStringEnumConverter = true
)]
[JsonSerializable(typeof(StorageMetadata))]
internal partial class StorageMetadataJsonSerializerContext : JsonSerializerContext;