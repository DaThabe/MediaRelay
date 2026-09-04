using MediaRelay.Http;
using Microsoft.Extensions.Configuration;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

public static class DependencyInjectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddHttpClient()
        {
             services.AddOptions<HttpOptions>()
                .PostConfigure<IConfiguration>((options, configuration) =>
                {
                    var selection = configuration.GetSection("Http");
                    selection.Bind(options);
                });

            services.AddSingleton<IHttpClient, MediaRelay.Http.HttpClient>();

            return services;
        }
    }
}
