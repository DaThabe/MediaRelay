using Microsoft.Extensions.Logging;

namespace MediaRelay.Messaging;


internal sealed class MessageSender<T>(
        IEnumerable<IMessageReceiver<T>> receivers,
        ILogger<MessageSender<T>> logger
    ) : IMessageSender<T>
{
    private readonly IMessageReceiver<T>[] _receivers = [.. receivers];

    public async ValueTask SendAsnc(T message, CancellationToken cancellationToken = default)
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
