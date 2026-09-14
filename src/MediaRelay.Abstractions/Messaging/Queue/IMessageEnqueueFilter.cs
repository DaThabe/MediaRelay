namespace MediaRelay.Messaging.Queue;

public interface IMessageEnqueueFilter<in TMessage, in TContent>
    where TMessage : IMessage<TContent>
{
    bool CanEnqueue(TMessage message);
}