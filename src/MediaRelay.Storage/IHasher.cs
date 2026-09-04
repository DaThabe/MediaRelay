using System.Security.Cryptography;

namespace MediaRelay.Storage;

public interface IHasher
{
    string Algorithm { get; }
    ValueTask<byte[]> HashAsync(Stream stream, CancellationToken cancellationToken = default);
}

internal sealed class SHA256Hasher : IHasher
{
    public string Algorithm { get; } = "SHA256";

    public ValueTask<byte[]> HashAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        return SHA256.HashDataAsync(stream, cancellationToken);
    }
}