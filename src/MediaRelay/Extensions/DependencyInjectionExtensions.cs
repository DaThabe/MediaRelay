using MediaRelay;
using MediaRelay.Content;
using MediaRelay.Input;
using MediaRelay.Publish;


#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

public static class DependencyInjectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMediaRelay()
        {
            services.AddSingleton<IMediaRelay, MediaRelay.MediaRelay>();
            services.AddSingleton<IInputParserSelector, InputParserSelector>();
            services.AddSingleton<IPublishContentConverterSelector, ContentHandlerSelector>();
            services.AddSingleton<IContentExtractorSelector, ContentExtractorSelector>();

            services.AddSingleton<IPublishOrchestrator, PublishOrchestrator>();

            return services;
        }
    }
}
