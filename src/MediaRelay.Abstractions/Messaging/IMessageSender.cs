using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace MediaRelay.Messaging;


public interface IMessageSender<TMessage, TContent>
    where TMessage : IMessage<TContent>
{
    /// <summary>
    /// 发送
    /// </summary>
    ValueTask SendAsync(TMessage message, CancellationToken cancellationToken = default);
}


public static class MessageSenderExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMessageSender<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TSender, TMessage, TContent>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TMessage : IMessage<TContent>
            where TSender : class, IMessageSender<TMessage, TContent>
        {
            var descriptor = new ServiceDescriptor(typeof(IMessageSender<TMessage, TContent>), typeof(TSender), lifetime);
            services.TryAddEnumerable(descriptor);

            return services;
        }
    }
}