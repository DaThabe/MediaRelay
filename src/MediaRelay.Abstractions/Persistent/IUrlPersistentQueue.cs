namespace MediaRelay.Persistent;


public interface IUrlPersistentQueue
{
    int Count { get; }

    ValueTask WriteAsync(Uri url, CancellationToken cancellationToken);
    ValueTask<Uri> ReadWaitAsync(CancellationToken cancellationToken);
    ValueTask<Uri> PeepWaitAsync(CancellationToken cancellationToken);
}