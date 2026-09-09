using MediaRelay;
using MediaRelay.Browser;
using MediaRelay.Content;
using MediaRelay.Http;
using MediaRelay.Logging;
using MediaRelay.Messaging;
using MediaRelay.Messaging.Queue;
using MediaRelay.Playwright;
using MediaRelay.Resources;
using MediaRelay.Source;
using MediaRelay.Storage;
using MediaRelay.Url;
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
            services
                .AddUrlRelay()
                .AddSourceRelay()
                .AddContentRelay()
                .AddRelay();

            return services;
        }
        private IServiceCollection AddUrlRelay()
        {
            services.AddSingleton<IUrlRelayService, UrlRelayService>();
            services.AddMessageQueue<UrlMessageQueue, UrlMessage, Uri>();
            services.AddHostedService<UrlQueueConsumer>();

            return services;
        }
        private IServiceCollection AddSourceRelay()
        {
            services.AddSingleton<IUrlSourceFactory, UrlSourceFactory>();
            services.AddSingleton<ISourceRelayService, SourceRelayService>();

            return services;
        }
        private IServiceCollection AddContentRelay()
        {
            // resource
            services.AddSingleton<IUrlResourceFactory, UrlResourceFactory>();

            // content
            services.AddSingleton<IContentExtractorFactory, ContentExtractorFactory>();
            services.AddSingleton<IContentRelayService, ContentRelayService>();

            return services;
        }
        private IServiceCollection AddRelay()
        {
            services.AddSingleton<IRelayContentFactory, RelayContentFactory>();
            services.AddSingleton<IRelay, Relay>();

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
                .AddMessaging();

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

            return services;
        }
        private IServiceCollection AddStorage()
        {
            services.AddOptions<StorageOptions>()
               .Configure<IConfiguration>((options, configuration) => configuration
                    .GetSection(StorageOptions.SectionPath)
                    .Bind(options));

            services.AddSingleton<IHasher, SHA256Hasher>();

            services.AddSingleton<IStorage, Storage>();
            services.AddSingleton<IResourceStorage, ResourceStorage>();

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