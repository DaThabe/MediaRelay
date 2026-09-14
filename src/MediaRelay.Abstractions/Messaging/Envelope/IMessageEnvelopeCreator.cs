namespace MediaRelay.Messaging.Queue;

public interface IMessageEnvelopeCreator<TEnvelope, TMessage, TContent>
    where TMessage : IMessage<TContent>
    where TEnvelope : IMessageEnvelope<TMessage, TContent>
{
    bool CanCreate(TMessage message);
    TEnvelope Create(TMessage message);
}