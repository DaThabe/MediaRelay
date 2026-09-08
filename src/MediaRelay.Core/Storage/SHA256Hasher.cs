using System.Security.Cryptography;

namespace MediaRelay.Storage;

internal sealed class SHA256Hasher : IHasher
{
    public string Algorithm { get; } = "SHA256";

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"/>
    public ValueTask<byte[]> HashAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        if (stream is null)
            throw new ArgumentNullException(nameof(stream), "Hash 数据流不可为空");

        stream.EnsureAtStart();
        return SHA256.HashDataAsync(stream, cancellationToken);
    }
}