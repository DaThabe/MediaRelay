using System.Text.Json;
using System.Text.Json.Serialization;

namespace MediaRelay.Immich.Json;


internal sealed class NumberStringConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
            return reader.GetDouble().ToString();
        if (reader.TokenType == JsonTokenType.String)
            return reader.GetString()!;
        return string.Empty;
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}