using MediaRelay.Messaging.Envelope;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MediaRelay.Messaging.Queue;


internal sealed class UrlMessageEnvelopePersistence(IOptions<UrlMessageQueueOptions> options) : IUrlMessageEnvelopePersistence
{
    public async ValueTask<IEnumerable<UrlMessageEnvelope>> LoadAsync(CancellationToken cancellationToken = default)
    {
        var filePath = options.Value.FilePath;
        if (!File.Exists(filePath)) return [];

        var json = await File.ReadAllTextAsync(filePath, cancellationToken);

        return JsonSerializer.Deserialize(json, UrlMessageQueueJsonSerializerContext.Default.UrlMessageEnvelopeArray) ?? [];
    }

    public async ValueTask SaveAsync(IEnumerable<UrlMessageEnvelope> envelopes, CancellationToken cancellationToken = default)
    {
        var filePath = options.Value.FilePath;
        var json = JsonSerializer.Serialize([.. envelopes], UrlMessageQueueJsonSerializerContext.Default.UrlMessageEnvelopeArray);

        var folder = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(folder)) Directory.CreateDirectory(folder);

        await File.WriteAllTextAsync(filePath, json, cancellationToken);
    }
}


[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    UseStringEnumConverter = true
)]
[JsonSerializable(typeof(UrlMessageEnvelope))]
[JsonSerializable(typeof(UrlMessageEnvelope[]))]
internal partial class UrlMessageQueueJsonSerializerContext : JsonSerializerContext;
