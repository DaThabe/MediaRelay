namespace MediaRelay.Storage.Hash;


public interface IHasher
{
    ValueTask<HashInfo> HashAsync(Stream stream, CancellationToken cancellationToken = default);
}