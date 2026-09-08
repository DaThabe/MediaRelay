using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;
namespace MediaRelay.Messaging;

public interface IMessageReceiver<TMessage>
{
    ValueTask OnReceivedAsync(TMessage message, Func<ValueTask>? ack = null, CancellationToken cancellationToken = default);
}


public static class MessageReceiverExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMessageReceiver<TMessage>(IMessageReceiver<TMessage> receiver, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            var descriptor = new ServiceDescriptor(typeof(IMessageReceiver<TMessage>), receiver, lifetime);
            services.TryAddEnumerable(descriptor);

            return services;
        }

        public IServiceCollection AddMessageReceiver<TMessage, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TReceiver>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TReceiver : class, IMessageReceiver<TMessage>
        {
            var descriptor = new ServiceDescriptor(typeof(IMessageReceiver<TMessage>), typeof(TReceiver), lifetime);
            services.TryAddEnumerable(descriptor);

            return services;
        }
    }
}