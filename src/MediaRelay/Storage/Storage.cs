using MediaRelay.Storage.Hash;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MediaRelay.Storage;


internal sealed class Storage(
    IOptions<StorageOptions> options,
    IHasher hasher,
    IStorageInfoRepository storageInfoRepository,
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
        var storageInfo = await storageInfoRepository.FindAsync(fileName, mediaType, cancellationToken);
        if (storageInfo is not null)
        {
            logger.LogInformation("已使用本地储存信息");
            return storageInfo;
        }

        storageInfo = await SaveMediaAsycn(stream, fileName, mediaType, cancellationToken);
        await storageInfoRepository.SetAsync(fileName, mediaType, storageInfo, cancellationToken);

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


    private string GetMediaFullPath(StorageFileName fileName, MediaType mediaType)
    {
        // 合并路径
        return Path.Combine(AppContext.BaseDirectory, options.Value.RootPath, $"{fileName}.{mediaType.Extensions}");
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