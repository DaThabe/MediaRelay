namespace MediaRelay.Storage;


public interface IStorage
{
    ValueTask<StorageInfo> StoreAsync(Stream stream, string extension, CancellationToken cancellationToken = default);
}
