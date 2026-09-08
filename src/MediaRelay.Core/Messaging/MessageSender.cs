using Microsoft.Extensions.Logging;

namespace MediaRelay.Messaging;


internal sealed class MessageSender<TMessage, TContent>(
        IMessageReceiverProvider messageReceiverProvider,
        ILogger<MessageSender<TMessage, TContent>> logger
    ) : IMessageSender<TMessage, TContent>
    where TMessage : IMessage<TContent>
{
    private readonly IMessageReceiver<TMessage, TContent>[] _receivers = [.. messageReceiverProvider.GetAll<TMessage, TContent>()];

    public async ValueTask SendAsync(TMessage message, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("开始发送消息");

        foreach (var receiver in _receivers)
        {
            using var _ = logger.BeginScope("ReceiverType", receiver.GetType().Name);

            try
            {
                await receiver.OnReceivedAsync(message, Ack, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "接受处理器发生错误");
            }


            ValueTask Ack()
            {
                logger.LogInformation("Receiver: {0}", receiver.GetType().Name);
                return ValueTask.CompletedTask;
            }
        }

        logger.LogInformation("消息发送完毕");
    }
}