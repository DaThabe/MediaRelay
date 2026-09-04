using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MediaRelay.Storage;


public interface IStorage
{
    ValueTask<StorageInfo> StoreAsync(Stream stream, string extension, CancellationToken cancellationToken = default);
}

internal sealed partial class Storage(
    IOptions<StorageOptions> options,
    IHasher hasher,
    ILogger<Storage> logger) : IStorage
{
    public async ValueTask<StorageInfo> StoreAsync(
        Stream stream,
        string extension,
        CancellationToken cancellationToken = default)
    {
        if (stream.Length == 0) throw new ArgumentException("流长度不可为空", nameof(stream));

        // Hash信息
        var (hashAlgorithm, hash) = await GetHashInfoAsync(stream, cancellationToken);
        // 完整路径
        var fullPath = await CombineFullPathAsync(hash, extension, cancellationToken);
        // 保存流
        var fullUri =  await SaveStreamToFileAsync(stream, fullPath, cancellationToken);

        return new StorageInfo()
        {
            Hash = hash,
            HashAlgorithm = hashAlgorithm,
            Uri = fullUri,
            Size = stream.Length,
            Extensions = extension
        };
    }

    private async Task<(string Algorithm, string Hash)> GetHashInfoAsync(Stream stream, CancellationToken cancellationToken)
    {
        // 重置流位置
        stream.EnsureAtStart();

        // 计算Hash
        var hashBytes = await hasher.HashAsync(stream, cancellationToken);
        var hash = Convert.ToHexString(hashBytes).ToLowerInvariant();

        return (hasher.Algorithm, hash);
    }

    private async Task<string> CombineFullPathAsync(string hash, string extension, CancellationToken cancellationToken)
    {
        // 格式化扩展名
        extension = extension.TrimStart('.');

        // 合并路径
        var fileName = $"{hash}.{extension}";
        return Path.Combine(AppContext.BaseDirectory, options.Value.Data.RootPath, fileName);
    }

    private async Task<Uri> SaveStreamToFileAsync(Stream source, string fullPath, CancellationToken cancellationToken)
    {
        var uri = new Uri($"file://{fullPath.Replace('\\', '/')}");
        Directory.CreateDirectory(options.Value.Data.RootPath);

        if (File.Exists(fullPath))
        {
            LogFileExisted(fullPath);
            return uri;
        }

        source.EnsureAtStart();
        await using var fs = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.Write, 4096);
        await source.CopyToAsync(fs, cancellationToken);

        LogFileSaved(fullPath);
        return uri;
    }


    [LoggerMessage(Level = LogLevel.Debug, Message = "文件已存在 < 路径 [{path}]")]
    private partial void LogFileExisted(string path);

    [LoggerMessage(Level = LogLevel.Debug, Message = "文件已保存 < 路径 [{path}]")]
    private partial void LogFileSaved(string path);
}