using MediaRelay;
using MediaRelay.Content;
using MediaRelay.Source.Url;
using MediaRelay.Twitter;
using MediaRelay.Twitter.Image;
using MediaRelay.Twitter.Tweet;
using Microsoft.Extensions.Configuration;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

public static class DependencyInjectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddTwitterSource()
        {
            services.AddOptions<TwitterOptions>()
                .PostConfigure<IConfiguration>((options, configuration) => configuration
                    .GetSection(TwitterOptions.Name)
                    .Bind(options));

            services.AddOptions<TwitterHttpOptions>()
                .PostConfigure<IConfiguration>((options, configuration) => configuration
                    .GetSection(TwitterOptions.Name)
                    .GetSection(TwitterHttpOptions.Name)
                    .Bind(options));

            services.AddOptions<TwitterImageUrlOptions>()
                .PostConfigure<IConfiguration>((options, configuration) => configuration
                    .GetSection(TwitterOptions.Name)
                    .GetSection(TwitterImageUrlOptions.Name)
                    .Bind(options));

            services.AddOptions<TwitterTweetOptions>()
                .PostConfigure<IConfiguration>((options, configuration) => configuration
                    .GetSection(TwitterOptions.Name)
                    .GetSection(TwitterTweetOptions.Name)
                    .Bind(options));



            services.AddSingleton<ImageUrl.Parser>();
            services.AddSingleton<ImageUrlResource.Factory>();

            services.AddSingleton<IUrlSourceParser, TweetSource.UrlParser>();
            services.AddSingleton<IContentExtractor, TweetContentExtractor>();
            services.AddSingleton<IRelayContentConverter, TweetPublishContentConverter>();


            return services;
        }
    }
}
