using MediaRelay;
using MediaRelay.Messaging;
using MediaRelay.Messaging.Queue;
using MediaRelay.Resources;
using MediaRelay.Source;
using MediaRelay.Url;
using MediaRelay.Url.Messaging.Queue;
using MediaRelay.Url.Resource;
using Microsoft.Extensions.Configuration;



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
            services.AddOptions<UrlOptions>()
                .Configure<IConfiguration>((options, configuration) => configuration
                     .GetSection(UrlOptions.SectionPath)
                     .Bind(options));

            services.AddOptions<UrlMessageQueueOptions>()
                .Configure<IConfiguration>((options, configuration) => configuration
                     .GetSection(UrlMessageQueueOptions.SectionPath)
                     .Bind(options));


            services.AddSingleton<IUrlSourceFactory, UrlSourceFactory>();
            services.AddSingleton<IUrlResourceFactory, UrlResourceFactory>();
            services.AddSingleton<IUrlRelayService, UrlRelayService>();

            services.AddMessageQueueWithSender<UrlMessageQueue, UrlMessage, Uri>();
            services.AddHostedService<UrlMessageQueueConsumer>();

            return services;
        }
    }
}