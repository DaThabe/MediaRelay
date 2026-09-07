using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MediaRelay.Storage;

internal sealed class Storage(
    IOptions<StorageOptions> options,
    IHasher hasher,
    ILogger<Storage> logger) : IStorage
{
    public async ValueTask<StorageInfo> StoreAsync(
        Stream stream,
        string extension,
        CancellationToken cancellationToken = default)
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
            var (hashAlgorithm, hash) = await GetHashInfoAsync(stream, cancellationToken);

            using var _ = logger.Scope("Hash", hash)
                .Add("HashAlgorithm", hashAlgorithm)
                .Begin();
            logger.LogInformation("文件Hash计算完成");

            // 完整路径
            var fullPath = CombineFullPath(hash, extension);
            // 保存流
            var fullUri = await SaveStreamToFileAsync(stream, fullPath, cancellationToken);

            return new StorageInfo()
            {
                Hash = hash,
                HashAlgorithm = hashAlgorithm,
                Uri = fullUri,
                Size = stream.Length,
                Extensions = extension
            };
        }
        finally
        {
            if (createdMemoryStream) await stream.DisposeAsync();
        }
    }

    private async Task<(string Algorithm, string Hash)> GetHashInfoAsync(Stream stream, CancellationToken cancellationToken)
    {
        // 重置流位置
        stream.EnsureAtStart();

        // 计算Hash
        var hash = await hasher.HashAsHexAsync(stream, cancellationToken);

        return (hasher.Algorithm, hash);
    }

    private string CombineFullPath(string hash, string extension)
    {
        // 格式化扩展名
        extension = extension.TrimStart('.');

        // 合并路径
        var fileName = $"{hash}.{extension}";
        return Path.Combine(AppContext.BaseDirectory, options.Value.RootPath, fileName);
    }

    private async Task<Uri> SaveStreamToFileAsync(Stream source, string fullPath, CancellationToken cancellationToken)
    {
        var uri = new Uri($"file://{fullPath.Replace('\\', '/')}");
        Directory.CreateDirectory(options.Value.RootPath);

        using var _ = logger.BeginScope("Uri", uri);

        if (File.Exists(fullPath))
        {
            logger.LogInformation("文件已存在");
            return uri;
        }

        source.EnsureAtStart();
        await using var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.Write, 4096, true);
        await source.CopyToAsync(fs, cancellationToken);

        logger.LogInformation("文件已储存");
        return uri;
    }
}