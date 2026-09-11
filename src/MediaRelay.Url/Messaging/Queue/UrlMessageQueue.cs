using MediaRelay.Messaging;
using MediaRelay.Messaging.Queue;
using MediaRelay.Source;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace MediaRelay.Url.Messaging.Queue;


internal sealed class UrlMessageQueue(
        IOptions<UrlOptions> options,
        IEnumerable<IUrlSourceParser> urlSourceParsers,
        ILogger<UrlMessageQueue> logger
    ) : PersistenceMessageQueue<UrlMessageEnvelope, UrlMessage, Uri>(logger), IMessageSender<UrlMessage, Uri>
{
    protected override bool CanEnqueue(UrlMessage message)
    {
        if (urlSourceParsers.Any(x => x.CanParse(message.Content)))
            return true;

        logger.LogInformation("该消息无法处理");
        return false;
    }

    protected override UrlMessageEnvelope CreateEnvelope(UrlMessage message)
    {
        return UrlMessageEnvelope.Create(message, UrlMessageRetryCounter.FromCount(options.Value.MessageQueue.MessageRetryCount));
    }
    protected override async ValueTask<IEnumerable<UrlMessageEnvelope>> LoadAsync(CancellationToken cancellationToken = default)
    {
        var filePath = options.Value.MessageQueue.File;
        if (!File.Exists(filePath)) return [];

        var json = await File.ReadAllTextAsync(filePath, cancellationToken);

        return JsonSerializer.Deserialize(json, UrlQueueJsonSerializerContext.Default.UrlMessageEnvelopeArray) ?? [];
    }
    protected override async ValueTask SaveAsync(IEnumerable<UrlMessageEnvelope> envelopes, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize([.. envelopes], UrlQueueJsonSerializerContext.Default.UrlMessageEnvelopeArray);

        var filePath = options.Value.MessageQueue.File;
        var folder = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(folder)) Directory.CreateDirectory(folder);

        await File.WriteAllTextAsync(filePath, json, cancellationToken);
    }
}