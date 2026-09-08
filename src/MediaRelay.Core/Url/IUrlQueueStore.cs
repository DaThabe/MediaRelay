using MediaRelay.Extensions;
using MediaRelay.Persistent.Url.Messages;
using MediaRelay.Url.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace MediaRelay.Url;


internal interface IUrlQueueStore
{
    ValueTask<Message[]> LoadAsync(CancellationToken cancellationToken = default);
    ValueTask SaveAsync(Message[] messages, CancellationToken cancellationToken = default);
}


internal sealed class FileUrlQueueStore(IOptions<MediaRelayOptions> options, ILogger<FileUrlQueueStore> logger) : IUrlQueueStore
{
    private readonly SemaphoreSlim _lock = new(1, 1);

    public async ValueTask<Message[]> LoadAsync(CancellationToken cancellationToken = default)
    {
        using var _ = await _lock.WaitScopeAsync(cancellationToken);

        // File
        var filePath = options.Value.UrlMessagesFile;
        using var __ = logger.BeginScope("FilePath", filePath);

        // Not Exists
        if (!File.Exists(filePath))
        {
            logger.LogWarning("网址消息文件不存在, 返回空消息集合");
            return [];
        }

        // Data
        logger.LogTrace("正在读取消息");
        var content = await File.ReadAllTextAsync(filePath, cancellationToken);
        var items = JsonSerializer.Deserialize(content, MessageJsonSerializerContext.Default.MessageArray);

        // Null
        if (items is null)
        {
            logger.LogWarning("不存在消息, 返回空消息集合");
            return [];
        }

        // Return
        logger.LogInformation("消息读取完成");
        return items;
    }

    public async ValueTask SaveAsync(Message[] items, CancellationToken cancellationToken = default)
    {
        using var _ = await _lock.WaitScopeAsync(cancellationToken);

        // Directory
        var filePath = options.Value.UrlMessagesFile;
        using var __ = logger.BeginScope("FilePath", filePath);

        var folder = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(folder)) Directory.CreateDirectory(folder);

        // Write
        logger.LogTrace("正在保存消息");
        var content = JsonSerializer.Serialize(items, MessageJsonSerializerContext.Default.MessageArray);
        await File.WriteAllTextAsync(filePath, content, cancellationToken);

        logger.LogInformation("消息保存完成");
    }
}