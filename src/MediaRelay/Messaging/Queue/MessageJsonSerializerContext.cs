using System.Text.Json.Serialization;

namespace MediaRelay.Messaging.Queue;


[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase
)]
[JsonSerializable(typeof(MessageEnvelope))]
[JsonSerializable(typeof(MessageEnvelope[]))]
[JsonSerializable(typeof(JsonStringEnumConverter<MessageEnvelopeStatus>))]
internal partial class QueueJsonSerializerContext : JsonSerializerContext;
