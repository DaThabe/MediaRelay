using MediaRelay.Storage.Hash;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MediaRelay.Storage.Media;


internal sealed class MediaRepository(
    IOptions<StorageOptions> options,
    IHasher hasher,
    IMediaStorageInfoRepository storageInfoRepository,
    ILogger<MediaRepository> logger) : IMediaRepository
{
    public ValueTask<bool> ExistsAsync(MediaType mediaType, StorageFileName fileName, CancellationToken cancellationToken = default)
    {
        var path = GetMediaFullPath(fileName, mediaType);
        return ValueTask.FromResult(File.Exists(path));
    }

    public async ValueTask<MediaStorageInfo> AddAsync(
        Stream stream,
        MediaType mediaType,
        StorageFileName fileName,
        CancellationToken cancellationToken = default)
    {
        var storageInfo = await storageInfoRepository.FindAsync(fileName, mediaType, cancellationToken);
        if (storageInfo is not null)
        {
            LogUseCacheMediaStorageInfo(storageInfo);
            return storageInfo;
        }

        storageInfo = await SaveMediaAsycn(stream, fileName, mediaType, cancellationToken);
        LogMediaStorageInfoSaved(storageInfo);

        await storageInfoRepository.SetAsync(fileName, mediaType, storageInfo, cancellationToken);

        return storageInfo;
    }


    private async Task<MediaStorageInfo> SaveMediaAsycn(Stream stream, StorageFileName fileName, MediaType mediaType, CancellationToken cancellationToken)
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
            // 完整路径
            var fullPath = GetMediaFullPath(fileName, mediaType);
            // 保存流
            var fullUri = await SaveStreamToFileAsync(stream, fullPath, cancellationToken);

            return new MediaStorageInfo()
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
    private static async Task<Uri> SaveStreamToFileAsync(Stream source, string fullPath, CancellationToken cancellationToken)
    {
        var uri = new Uri($"file://{fullPath.Replace('\\', '/')}");

        // 文件夹
        var folder = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrWhiteSpace(folder)) Directory.CreateDirectory(folder);

        // 文件存在
        if (File.Exists(fullPath)) return uri;

        // 保存
        source.EnsureAtStart();
        await using var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.Write, 4096, true);
        await source.CopyToAsync(fs, cancellationToken);

        // 储存路径
        return uri;
    }



    private void LogUseCacheMediaStorageInfo(MediaStorageInfo info)
    {
        if (!logger.IsEnabled(LogLevel.Information)) return;

        logger.LogInformation("已使用本地媒体储存信息, Type={Type}, Hash[{HashAlgorithm}]={HashHexString}, Size={Size}, Uri={Uri}",
            info.MediaType, info.HashInfo.Algorithm, info.HashInfo.HexString, info.Size, info.Uri);
    }
    private void LogMediaStorageInfoSaved(MediaStorageInfo info)
    {
        if (!logger.IsEnabled(LogLevel.Information)) return;

        logger.LogInformation("媒体已储存, Type={Type}, Hash[{HashAlgorithm}]={HashHexString}, Size={Size}, Uri={Uri}",
            info.MediaType, info.HashInfo.Algorithm, info.HashInfo.HexString, info.Size, info.Uri);
    }
}