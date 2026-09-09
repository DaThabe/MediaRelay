using MediaRelay;
using MediaRelay.Browser;
using MediaRelay.Content;
using MediaRelay.Http;
using MediaRelay.Logging;
using MediaRelay.Messaging;
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
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMediaRelay()
        {
            return services.AddCore()
                .AddHttpClient()
                .AddPlaywright()
                .AddStorage()
                .AddMessaging();
        }

        private IServiceCollection AddCore()
        {
            services.AddOptions<MediaRelayOptions>()
            .Configure<IConfiguration>((options, configuration) => configuration
                 .GetSection(MediaRelayOptions.SectionPath)
                 .Bind(options));

            services.AddSingleton<IUrlParserSelector, UrlParserSelector>();
            services.AddSingleton<IUrlSourceFactory, UrlSourceFactory>();
            services.AddSingleton<IContentConverterSelector, ContentHandlerSelector>();
            services.AddSingleton<IContentExtractorSelector, ContentExtractorSelector>();
            services.AddSingleton<IRelayOrchestrator, RelayOrchestrator>();
            services.AddSingleton<IUrlResourceFactory, UrlResourceFactory>();

            // 转发
            services.AddSingleton<IUrlRelayService, UrlRelayService>();
            services.AddSingleton<ISourceRelayService, SourceRelayService>();
            services.AddSingleton<IContentRelayService, ContentRelayService>();

            // 消息队列
            services.AddMessageQueue<UrlMessageQueue, UrlMessage, Uri>();
            services.AddMessageSender<UrlMessageSender, UrlMessage, Uri>();
            services.AddHostedService<UrlQueueConsumer>();

            return services;
        }

        private IServiceCollection AddMessaging()
        {
            services.AddSingleton<IMessageOrchestrator, MessageOrchestrator>();
            services.AddSingleton<IMessageSenderProvider, MessageSenderProvider>();
            services.AddSingleton<IMessageReceiverProvider, MessageReceiverProvider>();
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

        private IServiceCollection AddPlaywright()
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
