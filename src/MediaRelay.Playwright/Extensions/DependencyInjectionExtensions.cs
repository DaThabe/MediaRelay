using MediaRelay.Playwright;
using Microsoft.Extensions.Configuration;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddPlaywright()
        {
            services.AddOptions<PlaywrightOptions>()
               .Configure<IConfiguration>((options, configuration) => configuration
                    .GetSection(PlaywrightOptions.Name)
                    .Bind(options));

            services.AddSingleton<IPlaywrightService, PlaywrightService>();
            services.AddSingleton<IBrowserService, ChromiumBrowserService>();

            return services;
        }
    }
}
