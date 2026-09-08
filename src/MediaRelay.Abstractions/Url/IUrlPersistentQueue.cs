namespace MediaRelay.Url;


public interface IUrlPersistentQueue
{
    int Count { get; }

    ValueTask WriteAsync(Uri url, CancellationToken cancellationToken);
    ValueTask<Uri> ReadWaitAsync(CancellationToken cancellationToken);
    ValueTask<Uri> PeepWaitAsync(CancellationToken cancellationToken);
}


public interface IUriPublisher
{
    ValueTask PublishAsync(Uri uri, CancellationToken cancellationToken = default);
}

//public interface IUrlReceiver
//{
//    ValueTask<Message> ReceivedAsync(Uri uri, CancellationToken cancellationToken = default);
//}