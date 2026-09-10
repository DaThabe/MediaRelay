using MediaRelay.Messaging.Queue;
using MediaRelay.Url;
using MediaRelay.Url.Messaging.Queue;
using MediaRelay.Url.Resource;



#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

public static class DependencyInjectionExtensions
{
    // MediaRelay
    extension(IServiceCollection services)
    {
        public IServiceCollection AddUrl()
        {
            services.AddSingleton<IUrlSourceFactory, UrlSourceFactory>();
            services.AddSingleton<IUrlResourceFactory, UrlResourceFactory>();
            services.AddSingleton<IUrlRelayService, UrlRelayService>();

            services.AddMessageQueue<UrlMessageQueue, UrlMessage, Uri>();
            services.AddHostedService<UrlMessageQueueConsumer>();

            return services;
        }
    }
}