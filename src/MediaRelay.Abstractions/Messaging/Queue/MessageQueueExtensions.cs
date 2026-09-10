using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace MediaRelay.Messaging.Queue;

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