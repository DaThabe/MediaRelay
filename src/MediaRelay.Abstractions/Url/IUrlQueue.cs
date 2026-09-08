namespace MediaRelay.Url;


public interface IUrlQueue
{
    int Count { get; }

    ValueTask WriteAsync(Uri url, CancellationToken cancellationToken);
    ValueTask<Uri> ReadWaitAsync(CancellationToken cancellationToken);
    ValueTask<Uri> PeekWaitAsync(CancellationToken cancellationToken);
}


public interface IUriPublisher
{
    ValueTask PublishAsync(Uri uri, CancellationToken cancellationToken = default);
}
