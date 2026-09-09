using Microsoft.Extensions.DependencyInjection;

namespace MediaRelay.Messaging;


internal sealed class MessageOrchestrator(IServiceProvider serviceProvider) : IMessageOrchestrator
{
    public async ValueTask SendAsnc<TMessage, TContent>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : IMessage<TContent>
    {
        var sendTasks = serviceProvider
            .GetServices<IMessageSender<TMessage, TContent>>()
            .Select(x => x.SendAsync(message, cancellationToken).AsTask());

        await Task.WhenAll(sendTasks);
    }
}