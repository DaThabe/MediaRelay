namespace MediaRelay.Messaging;

public interface IMessageOrchestrator
{
    ValueTask SendAsnc<TMessage>(TMessage message, CancellationToken cancellationToken = default);
}