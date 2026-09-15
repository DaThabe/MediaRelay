using MediaRelay;
using MediaRelay.Browser;
using MediaRelay.Content;
using MediaRelay.Http;
using MediaRelay.Logging;
using MediaRelay.Messaging;
using MediaRelay.Payload;
using MediaRelay.Playwright;
using MediaRelay.Serialization;
using MediaRelay.Storage;
using MediaRelay.Storage.Hash;
using MediaRelay.Storage.Media;
using MediaRelay.Storage.Resource;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

public static class DependencyInjectionExtensions
{
    // MediaRelay
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMediaRelay()
        {
            // Config
            services.AddOptions<MediaRelayOptions>()
                .Configure<IConfiguration>((options, configuration) => configuration
                     .GetSection(MediaRelayOptions.SectionPath)
                     .Bind(options));

            return services
                .AddCore()
                .AddInfrastructure();
        }
    }

    // Core
    extension(IServiceCollection services)
    {
        private IServiceCollection AddCore()
        {
            // source
            services.AddSingleton<ISourceRelayService, SourceRelayService>();

            // content
            services.AddSingleton<IContentExtractorFactory, ContentExtractorFactory>();
            services.AddSingleton<IContentRelayService, ContentRelayService>();

            // relay
            services.AddSingleton<IPayloadFactory, PayloadFactory>();
            services.AddSingleton<IPayloadRelayService, PayloadRelayService>();

            return services;
        }
    }
    // Infrastructure
    extension(IServiceCollection services)
    {
        private IServiceCollection AddInfrastructure()
        {
            services
                .AddHttpClient()
                .AddBrowser()
                .AddStorage()
                .AddMessaging()
                .AddSerializer();

            return services;
        }
        private IServiceCollection AddMessaging()
        {
            services.AddSingleton<IMessageOrchestrator, MessageOrchestrator>();
            return services;
        }
        private IServiceCollection AddHttpClient()
        {
            services.AddOptions<HttpOptions>()
              .Configure<IConfiguration>((options, configuration) => configuration
                   .GetSection(HttpOptions.SectionPath)
                   .Bind(options));

            services.AddSingleton<IHttpClient, MediaRelay.Http.HttpClient>();

            return services;
        }
        private IServiceCollection AddBrowser()
        {
            services.AddOptions<BrowserOptions>()
               .Configure<IConfiguration>((options, configuration) => configuration
                    .GetSection(BrowserOptions.SectionPath)
                    .Bind(options));

            services.AddOptions<BrowserLaunchOptions>()
               .Configure<IConfiguration>((options, configuration) => configuration
                    .GetSection(BrowserLaunchOptions.SectionPath)
                    .Bind(options));


            services.AddSingleton<IPlaywrightService, PlaywrightService>();
            services.AddSingleton<IBrowserService, ChromiumBrowserService>();
            services.AddTransient<IPageSessionFactory, PageSessionFactory>();

            return services;
        }
        private IServiceCollection AddStorage()
        {
            services.AddOptions<StorageOptions>()
               .Configure<IConfiguration>((options, configuration) => configuration
                    .GetSection(StorageOptions.SectionPath)
                    .Bind(options));

            services.AddSingleton<IHasher, SHA256Hasher>();

            // Media
            services.AddSingleton<IMediaRepository, Storage>();
            services.AddSingleton<IMediaStorageInfoRepository, MediaStorageInfoRepository>();


            // Resource
            services.AddSingleton<IResourceFileNameFactory, ResourceFileNameFactory>();
            services.AddSingleton<IResourceRepository, ResourceStorage>();

            return services;
        }

        private IServiceCollection AddSerializer()
        {
            services.AddSingleton<IJsonSerializerFactory, JsonSerializerFactory>();
            return services;
        }
    }

    // Logger
    extension(ILoggingBuilder builder)
    {
        public ILoggingBuilder AddEmojiDebug()
        {
            builder.Services.TryAddInstanceEnumerable<ILoggerProvider>(EmojiLoggerProvider.Debug);
            return builder;
        }
        public ILoggingBuilder AddEmojiConsole()
        {
            builder.Services.TryAddInstanceEnumerable<ILoggerProvider>(EmojiLoggerProvider.Console);
            return builder;
        }
    }
}