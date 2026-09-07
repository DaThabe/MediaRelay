using MediaRelay;
using MediaRelay.Browser;
using MediaRelay.Content;
using MediaRelay.Http;
using MediaRelay.Logging;
using MediaRelay.Playwright;
using MediaRelay.Publish;
using MediaRelay.Source.Url;
using MediaRelay.Storage;
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
                .AddStorage();
        }

        private IServiceCollection AddCore()
        {
            services.AddSingleton<IUrlSourceParserSelector, UrlSourceParserSelector>();
            services.AddSingleton<IPublishContentConverterSelector, ContentHandlerSelector>();
            services.AddSingleton<IContentExtractorSelector, ContentExtractorSelector>();
            services.AddSingleton<IPublishOrchestrator, PublishOrchestrator>();

            services.AddSingleton<IMediaRelay, MediaRelay.MediaRelay>();

            return services;
        }

        private IServiceCollection AddHttpClient()
        {
            services.AddOptions<HttpOptions>()
              .Configure<IConfiguration>((options, configuration) => configuration
                   .GetSection(HttpOptions.Name)
                   .Bind(options));

            services.AddSingleton<IHttpClient, MediaRelay.Http.HttpClient>();

            return services;
        }

        private IServiceCollection AddPlaywright()
        {
            services.AddOptions<BrowserOptions>()
               .Configure<IConfiguration>((options, configuration) => configuration
                    .GetSection(BrowserOptions.Name)
                    .Bind(options));

            services.AddOptions<BrowserLaunchOptions>()
               .Configure<IConfiguration>((options, configuration) => configuration
                    .GetSection(BrowserOptions.Name)
                    .GetSection(BrowserLaunchOptions.Name)
                    .Bind(options));


            services.AddSingleton<IPlaywrightService, PlaywrightService>();
            services.AddSingleton<IBrowserService, ChromiumBrowserService>();

            return services;
        }

        private IServiceCollection AddStorage()
        {
            services.AddOptions<StorageOptions>()
               .Configure<IConfiguration>((options, configuration) => configuration
                    .GetSection(StorageOptions.Name)
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
            builder.Services.AddSingleton<ILoggerProvider>(EmojiLoggerProvider.Debug);
            return builder;
        }
        public ILoggingBuilder AddEmojiConsole()
        {
            builder.Services.AddSingleton<ILoggerProvider>(EmojiLoggerProvider.Console);
            return builder;
        }
    }
}
