namespace MediaRelay.Url;

public interface IUrlPersistentQueueFactory
{
    ValueTask<IUrlQueue> GetOrCreateAsync(CancellationToken cancellationToken = default);
}