namespace MediaRelay.Storage.Media;


public interface IMediaRepository
{
    ValueTask<bool> ExistsAsync(MediaType mediaType, StorageFileName fileName, CancellationToken cancellationToken = default);
    ValueTask<MediaStorageInfo> AddAsync(Stream stream, MediaType mediaType, StorageFileName fileName, CancellationToken cancellationToken = default);
}