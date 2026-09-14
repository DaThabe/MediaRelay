namespace MediaRelay.Storage.Hash;


public interface IHasher
{
    HashInfo Hash(ReadOnlySpan<byte> bytes);
    ValueTask<HashInfo> HashAsync(Stream stream, CancellationToken cancellationToken = default);
}