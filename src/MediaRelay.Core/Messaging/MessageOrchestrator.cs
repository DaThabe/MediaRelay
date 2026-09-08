namespace MediaRelay.Messaging;

internal sealed class MessageOrchestrator(IMessageSenderProvider messageSenderProvider) : IMessageOrchestrator
{
    public async ValueTask SendAsnc<TMessage, TContent>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : IMessage<TContent>
    {
        var tasks = messageSenderProvider.GetAll<TMessage, TContent>()
            .Select(x => x.SendAsync(message, cancellationToken).AsTask());

        await Task.WhenAll(tasks);
    }
}