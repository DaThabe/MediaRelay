using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace MediaRelay.Messaging;


public interface IMessageQueue<TMessage, TContent>
    where TMessage : IMessage<TContent>
{
    /// <summary>
    /// 入队
    /// </summary>
    ValueTask EnqueueAsync(TMessage message, CancellationToken cancellationToken = default);
    /// <summary>
    /// 出队最新的元素, 除非手动确认, 否则不会删除
    /// </summary>
    ValueTask<TMessage> DequeueAsync(CancellationToken cancellationToken = default);


    /// <summary>
    /// 确认
    /// </summary>
    ValueTask AcknowledgeAsync(TMessage message, CancellationToken cancellationToken = default);
    /// <summary>
    /// 拒绝
    /// </summary>
    ValueTask RejectAsync(TMessage message, bool requeue = true, CancellationToken cancellationToken = default);
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