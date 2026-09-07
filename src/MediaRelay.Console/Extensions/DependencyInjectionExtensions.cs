using MediaRelay.Console.Input;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

public static class DependencyInjectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMediaRelayConsole()
        {
            services.AddSingleton<IUrlPersistentQueue, UrlPersistentQueue>();
            services.AddHostedService<InputListenBackgroundService>();
            return services;
        }
    }
}
