using System.Text.Json.Serialization;

namespace MediaRelay.Url.Messaging.Queue;


[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    UseStringEnumConverter = true
)]
[JsonSerializable(typeof(UrlMessageEnvelope))]
[JsonSerializable(typeof(UrlMessageEnvelope[]))]
internal partial class UrlQueueJsonSerializerContext : JsonSerializerContext;
