namespace MediaRelay.Storage;

public interface IHasher
{
    string Algorithm { get; }
    ValueTask<byte[]> HashAsync(Stream stream, CancellationToken cancellationToken = default);
}

public static class HasherExtensions
{
    extension(IHasher hasher)
    {
        public async ValueTask<string> HashAsHexAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            var bytes = await hasher.HashAsync(stream, cancellationToken);
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }
    }
}