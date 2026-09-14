using MediaRelay.Storage.Hash;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MediaRelay.Storage;


internal sealed class Storage(
    IOptions<StorageOptions> options,
    IHasher hasher,
    ILogger<Storage> logger) : IStorage
{
    public ValueTask<bool> ExistsAsync(MediaType mediaType, StorageFileName fileName, CancellationToken cancellationToken = default)
    {
        var path = GetMediaFullPath(fileName, mediaType);
        return ValueTask.FromResult(File.Exists(path));
    }

    public async ValueTask<StorageInfo> StoreAsync(
        Stream stream,
        MediaType mediaType,
        StorageFileName fileName,
        CancellationToken cancellationToken = default)
    {

        var metadata = await LoadMetadataOrDefaultAsync(fileName, mediaType, cancellationToken);
        if (metadata is not null)
        {
            logger.LogInformation("已使用本地储存信息");
            return metadata;
        }

        var storageInfo = await SaveMediaAsycn(stream, fileName, mediaType, cancellationToken);
        await SaveMetadataAsync(storageInfo, fileName, mediaType, cancellationToken);

        return storageInfo;
    }

    private async Task<StorageInfo> SaveMediaAsycn(Stream stream, StorageFileName fileName, MediaType mediaType, CancellationToken cancellationToken)
    {
        bool createdMemoryStream = false;

        if (!stream.CanSeek)
        {
            stream = await stream.CopyToMemoryStreamAsync(cancellationToken);
            createdMemoryStream = true;
        }

        try
        {
            if (stream.Length == 0) throw new ArgumentException("流长度不可为空", nameof(stream));

            // Hash信息
            var hashInfo = await hasher.HashAsync(stream, cancellationToken);

            using var _ = logger.Scope("Hash", hashInfo.HexString)
                .Add("HashAlgorithm", hashInfo.Algorithm)
                .Begin();
            logger.LogInformation("文件Hash计算完成");

            // 完整路径
            var fullPath = GetMediaFullPath(fileName, mediaType);
            // 保存流
            var fullUri = await SaveStreamToFileAsync(stream, fullPath, cancellationToken);

            return new StorageInfo()
            {
                HashInfo = hashInfo,
                Uri = fullUri,
                Size = stream.Length,
                MediaType = mediaType
            };
        }
        finally
        {
            if (createdMemoryStream) await stream.DisposeAsync();
        }
    }

    private async Task<StorageInfo?> LoadMetadataOrDefaultAsync(StorageFileName fileName, MediaType mediaType, CancellationToken cancellationToken)
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

    private async Task SaveMetadataAsync(StorageInfo storageInfo, StorageFileName fileName, MediaType mediaType, CancellationToken cancellationToken)
    {
        var metadataFullPath = GetMetadataFullPath(fileName, mediaType);

        try
        {
            var metadata = new StorageMetadata()
            {
                HashAlgorithm = storageInfo.HashInfo.Algorithm,
                HashData = storageInfo.HashInfo.Data,
                Size = storageInfo.Size
            };

            var metadataString = JsonSerializer.Serialize(metadata, StorageMetadataJsonSerializerContext.Default.StorageMetadata);
            await File.WriteAllTextAsync(metadataFullPath, metadataString, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "无法保存元数据");
        }
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
    private async Task<Uri> SaveStreamToFileAsync(Stream source, string fullPath, CancellationToken cancellationToken)
    {
        var uri = new Uri($"file://{fullPath.Replace('\\', '/')}");
        using var _ = logger.BeginScope("Uri", uri);

        // 文件夹
        var folder = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrWhiteSpace(folder)) Directory.CreateDirectory(folder);

        // 文件存在
        if (File.Exists(fullPath))
        {
            logger.LogInformation("文件已存在");
            return uri;
        }

        // 保存
        source.EnsureAtStart();
        await using var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.Write, 4096, true);
        await source.CopyToAsync(fs, cancellationToken);

        // 储存路径
        logger.LogInformation("文件已储存");
        return uri;
    }
}


public record class StorageMetadata
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