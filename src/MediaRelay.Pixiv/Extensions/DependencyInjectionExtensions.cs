using MediaRelay;
using MediaRelay.Content;
using MediaRelay.Pixiv;
using MediaRelay.Pixiv.Artwork;
using MediaRelay.Pixiv.Image;
using MediaRelay.Source.Url;
using Microsoft.Extensions.Configuration;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

public static class DependencyInjectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddPixivSource()
        {
            services.AddOptions<PixivOptions>()
                .PostConfigure<IConfiguration>((options, configuration) => configuration
                    .GetSection(PixivOptions.Name)
                    .Bind(options));

            services.AddOptions<PixivArtworkOptions>()
                .PostConfigure<IConfiguration>((options, configuration) => configuration
                    .GetSection(PixivOptions.Name)
                    .GetSection(PixivArtworkOptions.Name)
                    .Bind(options));

            services.AddOptions<PixivHttpOptions>()
                .PostConfigure<IConfiguration>((options, configuration) => configuration
                    .GetSection(PixivOptions.Name)
                    .GetSection(PixivHttpOptions.Name)
                    .Bind(options));

            services.AddOptions<PixivOriginalImageUrlOptions>()
                .PostConfigure<IConfiguration>((options, configuration) => configuration
                    .GetSection(PixivOptions.Name)
                    .GetSection(PixivOriginalImageUrlOptions.Name)
                    .Bind(options));


            services.AddSingleton<OriginalImageUrl.Parser>();
            services.AddSingleton<OriginalImageUrlResource.Factory>();


            services.AddSingleton<IUrlSourceParser, ArtworkSource.UrlParser>();
            services.AddSingleton<IContentExtractor, ArtworkContentExtractor>();
            services.AddSingleton<IPixivDownloader, PixivDownloader>();
            services.AddSingleton<IRelayContentConverter, ArtworkPublishContentConverter>();


            return services;
        }
    }
}
