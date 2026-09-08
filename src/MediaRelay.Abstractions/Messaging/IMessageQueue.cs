using MediaRelay.Content;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace MediaRelay.Messaging;


public interface IMessageQueue<TMessage, TContent> : IMessageSender<TMessage, TContent>
    where TMessage : IMessage<TContent>
{
    /// <summary>
    /// 确认
    /// </summary>
    ValueTask AcknowledgeAsync(TMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// 拒绝
    /// </summary>
    ValueTask RejectAsync(TMessage message, bool requeue = true, CancellationToken cancellationToken = default);


    /// <summary>
    /// 阻塞接收
    /// </summary>
    ValueTask<TMessage> ReceiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 阻塞查看
    /// </summary>
    ValueTask<TMessage> PeekAsync(CancellationToken cancellationToken = default);
}


public static class MessageQueueExtensions
{
    extension(IServiceCollection services)
    {
        public void AddMessageQueue<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMessageQueue, TMessage, TContent>()
            where TMessage : IMessage<TContent>
            where TMessageQueue : class, IMessageQueue<TMessage, TContent>
        {
            services.AddSingleton<IMessageQueue<TMessage, TContent>, TMessageQueue>();
        }
    }
}