using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MediaRelay.Messaging;


internal sealed class MessageSender<T>(
    IServiceProvider services,
    ILogger<MessageSender<T>> logger
    ) : IMessageSender<T>
{
    private readonly IMessageReceiver<T>[] _receivers = [.. services.GetServices<IMessageReceiver<T>>()];

    public async ValueTask SendAsnc(T message, CancellationToken cancellationToken = default)
    {
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
    }
}