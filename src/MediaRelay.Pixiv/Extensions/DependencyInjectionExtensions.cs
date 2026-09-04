using MediaRelay.Content;
using MediaRelay.Input;
using MediaRelay.Pixiv;
using MediaRelay.Pixiv.Artworks;
using MediaRelay.Publish;
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
               .PostConfigure<IConfiguration>((options, configuration) =>
               {
                   var selection = configuration.GetSection("Pixiv");
                   selection.Bind(options);
               });

            services.AddSingleton<IPixivDownloader, PixivDownloader>();


            services.AddSingleton<IInputParser, PixivArtworksUrlSourceParser>();
            services.AddSingleton<IContentExtractor, PixivArtworkContentExtractor>();
            services.AddSingleton<IPublishContentConverter, PixivArtworkPublishContentConverter>();


            return services;
        }
    }
}
