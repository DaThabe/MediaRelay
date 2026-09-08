using MediaRelay.Messaging;

namespace MediaRelay.Url;

internal sealed class UrlMessageSender(IMessageQueue<InputUrlMessage, Uri> queue) : IMessageSender<InputUrlMessage, Uri>
{
    public ValueTask SendAsync(InputUrlMessage message, CancellationToken cancellationToken = default)
    {
        return queue.SendAsync(message, cancellationToken);
    }
}