using MediaRelay;
using MediaRelay.Content;
using MediaRelay.HeyBox;
using MediaRelay.HeyBox.BbsLink;
using MediaRelay.HeyBox.Image;
using Microsoft.Extensions.Configuration;
using MediaRelay.HeyBox;
using MediaRelay.Source;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

public static class DependencyInjectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddXiaoHeiHe()
        {
            services.AddOptions<XiaoHeiHeOptions>()
                .Configure<IConfiguration>((options, configuration) => configuration
                    .GetSection(XiaoHeiHeOptions.SectionPath)
                    .Bind(options));

            services.AddOptions<PixivArtworkOptions>()
                .Configure<IConfiguration>((options, configuration) => configuration
                    .GetSection(PixivArtworkOptions.SectionPath)
                    .Bind(options));

            services.AddOptions<PixivHttpOptions>()
                .Configure<IConfiguration>((options, configuration) => configuration
                    .GetSection(PixivHttpOptions.SectionPath)
                    .Bind(options));

            services.AddOptions<PixivOriginalImageUrlOptions>()
                .Configure<IConfiguration>((options, configuration) => configuration
                    .GetSection(PixivOriginalImageUrlOptions.SectionPath)
                    .Bind(options));


            services.AddSingleton<OriginalImageUrl.Parser>();
            services.AddSingleton<OriginalImageUrlResource.Factory>();


            services.AddSingleton<IUrlSourceParser, LinkSource.UrlParser>();
            services.AddSingleton<IContentExtractor, ArtworkContentExtractor>();
            services.AddSingleton<IPixivDownloader, PixivDownloader>();
            services.AddSingleton<IRelayContentCreator, ArtworkPublishContentConverter>();


            return services;
        }
    }
}
