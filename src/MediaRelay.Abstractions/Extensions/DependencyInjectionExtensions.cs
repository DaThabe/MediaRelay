using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;


#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

public static class DependencyInjectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection TryAddEnumerable<TService,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(ServiceLifetime lifetime)
            where TService : class
            where TImplementation : class, TService
        {
            var descriptor = new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime);
            services.TryAddEnumerable(descriptor);

            return services;
        }

        public IServiceCollection TryAddSingleEnumerable<TService,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            return services.TryAddEnumerable<TService, TImplementation>(ServiceLifetime.Singleton);
        }


        public IServiceCollection TryAddEnumerable<TService>(TService instance, ServiceLifetime lifetime)
            where TService : class
        {
            var descriptor = new ServiceDescriptor(typeof(TService), instance, lifetime);
            services.TryAddEnumerable(descriptor);

            return services;
        }
        public IServiceCollection TryAddSingleEnumerable<TService>(TService instance) where TService : class =>
            services.TryAddEnumerable(instance, ServiceLifetime.Singleton);
    }
}
