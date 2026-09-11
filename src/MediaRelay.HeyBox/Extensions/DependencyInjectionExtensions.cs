using MediaRelay;
using MediaRelay.Content;
using MediaRelay.HeyBox;
using MediaRelay.HeyBox.BbsLink;
using MediaRelay.HeyBox.Image;
using Microsoft.Extensions.Configuration;
using MediaRelay.Source;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

public static class DependencyInjectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddHeyBox()
        {
            services.AddOptions<HeyBoxOptions>()
                .Configure<IConfiguration>((options, configuration) => configuration
                    .GetSection(HeyBoxOptions.SectionPath)
                    .Bind(options));

            services.AddOptions<HeyBoxBbsLinkOptions>()
                .Configure<IConfiguration>((options, configuration) => configuration
                    .GetSection(HeyBoxBbsLinkOptions.SectionPath)
                    .Bind(options));

            services.AddOptions<HeyBoxHttpOptions>()
                .Configure<IConfiguration>((options, configuration) => configuration
                    .GetSection(HeyBoxHttpOptions.SectionPath)
                    .Bind(options));

            services.AddOptions<HeyBoxOriginalImageUrlOptions>()
                .Configure<IConfiguration>((options, configuration) => configuration
                    .GetSection(HeyBoxOriginalImageUrlOptions.SectionPath)
                    .Bind(options));


            services.AddSingleton<OriginalImageUrl.Parser>();
            services.AddSingleton<OriginalImageUrlResource.Factory>();


            services.AddSingleton<IUrlSourceParser, LinkSource.UrlParser>();
            services.AddSingleton<IContentExtractor, LinkContentExtractor>();
            services.AddSingleton<IRelayContentCreator, LinkPublishContentConverter>();


            return services;
        }
    }
}
