namespace MediaRelay.Messaging;

public interface IMessageSender<TMessage>
{
    ValueTask SendAsnc(TMessage message, CancellationToken cancellationToken = default);
}