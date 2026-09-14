using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace MediaRelay.Messaging.Queue;


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
    ValueTask AcknowledgeAsync(MessageId messageId, CancellationToken cancellationToken = default);
    /// <summary>
    /// 拒绝
    /// </summary>
    ValueTask RejectAsync(MessageId messageId, CancellationToken cancellationToken = default);
}


public static class MessageQueueExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// 注册发送者队列 <see cref="IMessageQueue{TMessage, TContent}"/> And  <see cref="IMessageSender{TMessage, TContent}"/>`1
        /// </summary>
        /// <typeparam name="TMessageQueue">实际消息队列类型</typeparam>
        /// <typeparam name="TMessage">消息类型</typeparam>
        /// <typeparam name="TContent">消息内容类型</typeparam>
        public void AddMessageQueueWithSender<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMessageQueue, TMessage, TContent>()
            where TMessage : IMessage<TContent>
            where TMessageQueue : class, IMessageQueue<TMessage, TContent>, IMessageSender<TMessage, TContent>
        {
            services.AddSingleton<TMessageQueue>();
            services.AddSingleton<IMessageQueue<TMessage, TContent>>(sp => sp.GetRequiredService<TMessageQueue>());
            services.AddSingleton<IMessageSender<TMessage, TContent>>(sp => sp.GetRequiredService<TMessageQueue>());
        }
    }
}