namespace MediaRelay.Storage;


public interface IStorageInfoRepository
{
    ValueTask<StorageInfo?> FindAsync(StorageFileName fileName, MediaType mediaType, CancellationToken cancellationToken);
    ValueTask SetAsync(StorageFileName fileName, MediaType mediaType, StorageInfo storageInfo, CancellationToken cancellationToken);
}