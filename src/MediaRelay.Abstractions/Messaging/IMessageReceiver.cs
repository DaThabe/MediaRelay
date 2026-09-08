using MediaRelay.Content;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace MediaRelay.Messaging;


public interface IMessageReceiver<TMessage, TContent>
    where TMessage : IMessage<TContent>
{
    ValueTask OnReceivedAsync(TMessage message, Func<ValueTask>? ack = null, CancellationToken cancellationToken = default);
}

public static class MessageReceiverExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMessageReceiver<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TReceiver, TMessage, TContent>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TMessage : IMessage<TContent>
            where TReceiver : class, IMessageReceiver<TMessage, TContent>
        {
            var descriptor = new ServiceDescriptor(typeof(IMessageReceiver<TMessage, TContent>), typeof(TReceiver), lifetime);
            services.TryAddEnumerable(descriptor);

            return services;
        }
    }
}