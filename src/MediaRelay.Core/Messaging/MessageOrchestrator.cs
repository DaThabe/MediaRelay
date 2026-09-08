using Microsoft.Extensions.DependencyInjection;

namespace MediaRelay.Messaging;

internal sealed class MessageOrchestrator(
        IServiceProvider services
    ) : IMessageOrchestrator
{
    public async ValueTask SendAsnc<TMessage>(TMessage message, CancellationToken cancellationToken = default)
    {
        var tasks = services.GetServices<IMessageSender<TMessage>>()
            .Select(x => x.SendAsnc(message, cancellationToken).AsTask());

        await Task.WhenAll(tasks);
    }
}