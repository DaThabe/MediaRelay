using Microsoft.Extensions.DependencyInjection;

namespace MediaRelay.Messaging;

internal sealed class MessageSenderProvider(IServiceProvider serviceProvider) : IMessageSenderProvider
{
    public IEnumerable<IMessageSender<TMessage, TContent>> GetAll<TMessage, TContent>() 
        where TMessage : IMessage<TContent>
    {
        return serviceProvider.GetServices<IMessageSender<TMessage, TContent>>() ?? [];
    }
}