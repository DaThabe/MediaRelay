namespace MediaRelay.Persistent;

public interface IUrlPersistentQueueFactory
{
    ValueTask<IUrlPersistentQueue> GetOrCreateAsync(CancellationToken cancellationToken = default);
}