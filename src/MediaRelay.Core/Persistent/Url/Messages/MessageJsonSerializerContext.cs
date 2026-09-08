using System.Text.Json.Serialization;

namespace MediaRelay.Persistent.Url.Messages;


[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase
)]
[JsonSerializable(typeof(Message))]
[JsonSerializable(typeof(Message[]))]
internal partial class MessageJsonSerializerContext : JsonSerializerContext;