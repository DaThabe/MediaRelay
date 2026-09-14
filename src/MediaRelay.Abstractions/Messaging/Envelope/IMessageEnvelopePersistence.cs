namespace MediaRelay.Messaging.Queue;


/// <summary>
/// 信封持久化业务
/// </summary>
public interface IMessageEnvelopePersistence<TEnvelope, in TMessage, in TContent>
    where TEnvelope : IMessageEnvelope<TMessage, TContent>
    where TMessage : IMessage<TContent>
{
    ValueTask<IEnumerable<TEnvelope>> LoadAsync(CancellationToken cancellationToken = default);
    ValueTask SaveAsync(IEnumerable<TEnvelope> envelopes, CancellationToken cancellationToken = default);
}