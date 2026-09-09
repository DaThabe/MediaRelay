using Microsoft.Extensions.DependencyInjection;

namespace MediaRelay.Messaging;

internal sealed class MessageReceiverProvider(IServiceProvider serviceProvider) : IMessageReceiverProvider
{
    public IEnumerable<IMessageReceiver<TMessage, TContent>> GetAll<TMessage, TContent>()
        where TMessage : IMessage<TContent>
    {
        return serviceProvider.GetServices<IMessageReceiver<TMessage, TContent>>() ?? [];
    }
}