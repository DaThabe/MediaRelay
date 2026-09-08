namespace MediaRelay.Messaging;

public interface IMessageReceiverProvider
{
    IEnumerable<IMessageReceiver<TMessage, TContent>> GetAll<TMessage, TContent>()
        where TMessage : IMessage<TContent>;
}

public interface IMessageSenderProvider
{
    IEnumerable<IMessageSender<TMessage, TContent>> GetAll<TMessage, TContent>()
        where TMessage : IMessage<TContent>;
}
