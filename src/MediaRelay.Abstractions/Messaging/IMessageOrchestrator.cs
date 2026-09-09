namespace MediaRelay.Messaging;


public interface IMessageOrchestrator
{
    ValueTask SendAsnc<TMessage, TContent>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : IMessage<TContent>;
}