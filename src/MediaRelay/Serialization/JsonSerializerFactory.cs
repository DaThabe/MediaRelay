using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace MediaRelay.Serialization;


internal sealed class JsonSerializerFactory : IJsonSerializerFactory
{
    public ISerializer<T> Create<T>(JsonTypeInfo<T> jsonTypeInfo) where T : notnull
    {
        return new JsonSerializer<T>(jsonTypeInfo);
    }
}

file sealed class JsonSerializer<T>(JsonTypeInfo<T> jsonTypeInfo) : ISerializer<T>
    where T : notnull
{
    public async ValueTask<T> DeserializeAsync(Stream source, CancellationToken cancellationToken = default)
    {
        return await JsonSerializer.DeserializeAsync(source, jsonTypeInfo, cancellationToken) ??
            throw new JsonException("反序列化结果为 null，数据可能损坏");
    }

    public async ValueTask SerializeAsync(Stream destination, T data, CancellationToken cancellationToken = default)
    {
        await JsonSerializer.SerializeAsync(destination, data, jsonTypeInfo, cancellationToken);
    }
}