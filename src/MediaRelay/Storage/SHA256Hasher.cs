using System.Security.Cryptography;

namespace MediaRelay.Storage;

internal sealed class SHA256Hasher : IHasher
{
    public string Algorithm { get; } = "SHA256";

    public ValueTask<byte[]> HashAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        return SHA256.HashDataAsync(stream, cancellationToken);
    }
}