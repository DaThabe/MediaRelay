namespace MediaRelay.Messaging;

public interface IMessageReceiver<TMessage>
{
    ValueTask OnReceivedAsync(TMessage message, Func<ValueTask>? ack = null, CancellationToken cancellationToken = default);
}