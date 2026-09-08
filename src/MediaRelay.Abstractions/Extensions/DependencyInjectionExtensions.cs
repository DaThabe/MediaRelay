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
            var descriptor = new ServiceDescriptor(
                serviceType: typeof(TService),
                implementationType: typeof(TImplementation),
                lifetime: lifetime);
            services.TryAddEnumerable(descriptor);

            return services;
        }

        public IServiceCollection TryAddSingleEnumerable<TService,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            var descriptor = new ServiceDescriptor(
                serviceType: typeof(TService),
                implementationType: typeof(TImplementation),
                lifetime: ServiceLifetime.Singleton);
            services.TryAddEnumerable(descriptor);

            return services;
        }


        public IServiceCollection TryAddInstanceEnumerable<TService>(TService instance)
            where TService : class
        {
            var descriptor = new ServiceDescriptor(
                serviceType: typeof(TService),
                instance: instance);
            services.TryAddEnumerable(descriptor: descriptor);

            return services;
        }
    }
}
