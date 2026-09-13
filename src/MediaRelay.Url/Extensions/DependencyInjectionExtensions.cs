using MediaRelay;
using MediaRelay.Messaging;
using MediaRelay.Messaging.Queue;
using MediaRelay.Payload;
using MediaRelay.Resource;
using MediaRelay.Source;
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

            

            // Source
            services.AddSingleton<IUrlSourceFactory, UrlSourceFactory>();
            // Reource
            services.AddSingleton<IUrlResourceFactory, UrlResourceFactory>();
            // Relay
            services.AddSingleton<IUrlRelayService, UrlRelayService>();
            // Payload
            services.AddSingleton<IPayloadCreator, DefaultUrlPayloadCreator>();


            // Messaging
            services.AddOptions<UrlMessageQueueOptions>()
                .Configure<IConfiguration>((options, configuration) => configuration
                     .GetSection(UrlMessageQueueOptions.SectionPath)
                     .Bind(options));

            services.AddMessageQueueWithSender<UrlMessageQueue, UrlMessage, Uri>();
            services.AddHostedService<UrlMessageQueueConsumer>();


            return services;
        }
    }
}