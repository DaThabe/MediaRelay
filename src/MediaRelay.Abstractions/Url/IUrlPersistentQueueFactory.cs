namespace MediaRelay.Url;

public interface IUrlPersistentQueueFactory
{
    ValueTask<IUrlPersistentQueue> GetOrCreateAsync(CancellationToken cancellationToken = default);
}