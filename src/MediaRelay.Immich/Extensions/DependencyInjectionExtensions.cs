using Apigen.Immich.Client;
using MediaRelay.Immich;
using MediaRelay.Publish;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

public static class DependencyInjectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddImmichDestination()
        {
            services.AddOptions<ImmichOptions>()
               .PostConfigure<IConfiguration>((options, configuration) =>
               {
                   var selection = configuration.GetSection("Immich");
                   selection.Bind(options);
               });

            services.AddSingleton(sp =>
            {
                ApigenImmichClientHacker.Hack();

                var options = sp.GetRequiredService<IOptions<ImmichOptions>>();
                var logger = sp.GetRequiredService<ILogger<ImmichApiClient>>();

                return ImmichApiClient.WithApiKey(options.Value.ApiKey, options.Value.BaseUrl, logger: logger);
            });

            services.AddSingleton<IPublishExecutor, ImmichPublishHandler>();


            return services;
        }
    }


    
}
