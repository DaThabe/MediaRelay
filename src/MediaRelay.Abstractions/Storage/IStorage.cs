namespace MediaRelay.Storage;


public interface IStorage
{
    ValueTask<StorageInfo> StoreAsync(Stream stream, MediaType format, CancellationToken cancellationToken = default);
}
