using MediaRelay.Messaging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MediaRelay.Url;


internal sealed class UrlMessageQueue(IOptions<MediaRelayOptions> options) : PersistenceMessageQueue<InputUrlMessage, Uri>
{
    protected override IMessageEnvelope<InputUrlMessage, Uri> CreateEnvelope(InputUrlMessage message)
    {
        return new MessageEnvelope() { Message = message };
    }

    protected override async ValueTask<IEnumerable<IMessageEnvelope<InputUrlMessage, Uri>>> LoadAsync(CancellationToken cancellationToken = default)
    {
        var filePath = options.Value.UrlMessageQueueFile;
        if (!File.Exists(filePath)) return [];
        var json = await File.ReadAllTextAsync(filePath, cancellationToken);

        return JsonSerializer.Deserialize(json, MessageJsonSerializerContext.Default.MessageEnvelopeArray) ?? [];
    }

    protected override async ValueTask SaveAsync(IEnumerable<IMessageEnvelope<InputUrlMessage, Uri>> data, CancellationToken cancellationToken = default)
    {
        var dto = data.Select(x => new MessageEnvelope() { Message = x.Message, CreateAt = x.CreateAt, Status = x.Status }).ToArray();
        var json = JsonSerializer.Serialize(dto, MessageJsonSerializerContext.Default.MessageEnvelopeArray);

        var filePath = options.Value.UrlMessageQueueFile;
        var folder = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(folder)) Directory.CreateDirectory(folder);

        await File.WriteAllTextAsync(filePath, json, cancellationToken);
    }
}

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase
)]
[JsonSerializable(typeof(MessageEnvelope))]
[JsonSerializable(typeof(MessageEnvelope[]))]
[JsonSerializable(typeof(JsonStringEnumConverter<MessageStatus>))]
internal partial class MessageJsonSerializerContext : JsonSerializerContext;


internal sealed class MessageEnvelope : IMessageEnvelope<InputUrlMessage, Uri>
{
    public required InputUrlMessage Message { get; init; }
    public MessageStatus Status { get; init; } = MessageStatus.Pending;
    public DateTimeOffset CreateAt { get; init; } = DateTimeOffset.Now;
}