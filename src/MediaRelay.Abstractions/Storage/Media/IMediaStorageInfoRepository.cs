namespace MediaRelay.Storage.Media;


public interface IMediaStorageInfoRepository
{
    ValueTask<MediaStorageInfo?> FindAsync(StorageFileName fileName, MediaType mediaType, CancellationToken cancellationToken);
    ValueTask SetAsync(StorageFileName fileName, MediaType mediaType, MediaStorageInfo storageInfo, CancellationToken cancellationToken);
}