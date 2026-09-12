using MediaRelay.Messaging;
using System.Text.Json.Serialization;

namespace MediaRelay.Messaging.Queue;


[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    UseStringEnumConverter = true
)]
[JsonSerializable(typeof(UrlMessageEnvelope))]
[JsonSerializable(typeof(UrlMessageEnvelope[]))]
internal partial class UrlQueueJsonSerializerContext : JsonSerializerContext;
