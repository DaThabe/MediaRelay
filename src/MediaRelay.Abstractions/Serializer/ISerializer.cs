using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace MediaRelay.Serializer;


/// <summary>
/// 序列化器
/// </summary>
public interface ISerializer<T>
    where T : notnull
{
    ValueTask<T> DeserializeAsync(Stream source, CancellationToken cancellationToken = default);
    ValueTask SerializeAsync(Stream destination, T data, CancellationToken cancellationToken = default);
}

public static class SerializerExtensions
{
    extension<T>(ISerializer<T> serializer)
        where T : notnull
    {
        public async ValueTask<T> DeserializeFromStringAsync(string source, Encoding encoding, CancellationToken cancellationToken = default)
        {
            var bytes = encoding.GetBytes(source);
            await using var ms = new MemoryStream(bytes);

            return await serializer.DeserializeAsync(ms, cancellationToken);
        }

        public async ValueTask<string> SerializeToStringAsync(T data, Encoding encoding, CancellationToken cancellationToken = default)
        {
            await using var ms = new MemoryStream();
            await serializer.SerializeAsync(ms, data, cancellationToken);

            return encoding.GetString(ms.GetBuffer(), 0, (int)ms.Length);
        }


        public ValueTask<T> DeserializeFromUTF8StringAsync(string source, CancellationToken cancellationToken = default)
            => serializer.DeserializeFromStringAsync(source, Encoding.UTF8, cancellationToken);

        public ValueTask<string> SerializeToUTF8StringAsync(T data, CancellationToken cancellationToken = default)
            => serializer.SerializeToStringAsync(data, Encoding.UTF8, cancellationToken);
    }
}