using System.Security.Cryptography;

namespace MediaRelay.Storage.Hash;


internal sealed class SHA256Hasher : IHasher
{
    public async ValueTask<HashInfo> HashAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        if (stream is null)
            throw new ArgumentNullException(nameof(stream), "Hash 数据流不可为空");

        stream.EnsureAtStart();
        var data = await SHA256.HashDataAsync(stream, cancellationToken);

        return HashInfo.FromSHA256(data);
    }

    public HashInfo Hash(ReadOnlySpan<byte> bytes)
    {
        if (bytes.IsEmpty)
            throw new ArgumentException("Hash 数据流不可为空", nameof(bytes));

        var data = SHA256.HashData(bytes);

        return HashInfo.FromSHA256(data);
    }
}