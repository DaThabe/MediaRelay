namespace MediaRelay.Storage;

public interface IHasher
{
    string Algorithm { get; }
    ValueTask<byte[]> HashAsync(Stream stream, CancellationToken cancellationToken = default);
}
