using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace MediaRelay.Messaging.Queue;


public interface IMessageQueue<TMessage, TContent> : IMessageSender<TMessage, TContent>
    where TMessage : IMessage<TContent>
{
    ValueTask IMessageSender<TMessage, TContent>.SendAsync(TMessage message, CancellationToken cancellationToken) =>
        EnqueueAsync(message, cancellationToken);

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
    ValueTask RejectAsync(TMessage message, CancellationToken cancellationToken = default);
}


public static class MessageQueueExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// 注册队列 <see cref="IMessageQueue{TMessage, TContent}"/> 和 <see cref="IMessageSender{TMessage, TContent}"/>
        /// </summary>
        /// <typeparam name="TMessageQueue">实际消息队列类型</typeparam>
        /// <typeparam name="TMessage">消息类型</typeparam>
        /// <typeparam name="TContent">消息内容类型</typeparam>
        public void AddMessageQueue<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMessageQueue, TMessage, TContent>()
            where TMessage : IMessage<TContent>
            where TMessageQueue : class, IMessageQueue<TMessage, TContent>
        {
            services.TryAddSingleEnumerable<IMessageQueue<TMessage, TContent>, TMessageQueue>();
            services.AddSingleton<IMessageSender<TMessage, TContent>>(x => x.GetRequiredService<IMessageQueue<TMessage, TContent>>());
        }
    }
}