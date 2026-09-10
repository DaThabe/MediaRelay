namespace MediaRelay.Messaging.Queue;


public interface IMessageEnvelope<TMessage, out TContent>
    where TMessage : IMessage<TContent>
{
    TMessage Message { get; }
    MessageEnvelopeStatus Status { get; }
    DateTimeOffset CreateAt { get; }


    void MarkProcessing();
    void MarkCompleted();
    void MarkRejected();


    bool TryRetry();
    void Recover();
}