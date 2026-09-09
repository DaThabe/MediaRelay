using MediaRelay.Url;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace MediaRelay.Messaging.Queue;


internal sealed class UrlMessageQueue(IOptions<MediaRelayOptions> options, ILogger<UrlMessageQueue> logger) :
    PersistenceMessageQueue<MessageEnvelope, UrlMessage, Uri>(logger)
{
    protected override MessageEnvelope CreateEnvelope(UrlMessage message)
    {
        return MessageEnvelope.Create(message, MessageRetryOptions.FromCount(5));
    }
    protected override async ValueTask<IEnumerable<MessageEnvelope>> LoadAsync(CancellationToken cancellationToken = default)
    {
        var filePath = options.Value.UrlMessageQueueFile;
        if (!File.Exists(filePath)) return [];
        var json = await File.ReadAllTextAsync(filePath, cancellationToken);

        return JsonSerializer.Deserialize(json, QueueJsonSerializerContext.Default.MessageEnvelopeArray) ?? [];
    }
    protected override async ValueTask SaveAsync(IEnumerable<MessageEnvelope> envelopes, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize([.. envelopes], QueueJsonSerializerContext.Default.MessageEnvelopeArray);

        var filePath = options.Value.UrlMessageQueueFile;
        var folder = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(folder)) Directory.CreateDirectory(folder);

        await File.WriteAllTextAsync(filePath, json, cancellationToken);
    }
}