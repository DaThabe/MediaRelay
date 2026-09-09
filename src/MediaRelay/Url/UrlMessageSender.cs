using MediaRelay.Messaging;

namespace MediaRelay.Url;

internal sealed class UrlMessageSender(IMessageQueue<UrlMessage, Uri> queue) : IMessageSender<UrlMessage, Uri>
{
    public ValueTask SendAsync(UrlMessage message, CancellationToken cancellationToken = default)
    {
        return queue.EnqueueAsync(message, cancellationToken);
    }
}