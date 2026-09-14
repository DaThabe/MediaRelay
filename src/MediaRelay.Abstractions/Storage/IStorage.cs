namespace MediaRelay.Storage;


public interface IStorage
{
    ValueTask<bool> ExistsAsync(MediaType mediaType, StorageFileName fileName, CancellationToken cancellationToken = default);
    ValueTask<StorageInfo> StoreAsync(Stream stream, MediaType mediaType, StorageFileName fileName, CancellationToken cancellationToken = default);
}