using Apigen.Immich.Client;
using MediaRelay;
using MediaRelay.Immich;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddImmich()
        {
            services.AddOptions<ImmichOptions>()
                .Configure<IConfiguration>((options, configuration) => configuration
                    .GetSection(ImmichOptions.SectionPath)
                    .Bind(options));

            services.AddSingleton(sp =>
            {
                ApigenImmichClientHacker.Hack();

                var options = sp.GetRequiredService<IOptions<ImmichOptions>>();
                var logger = sp.GetRequiredService<ILogger<ImmichApiClient>>();

                return ImmichApiClient.WithApiKey(options.Value.ApiKey, options.Value.BaseUrl, logger: logger);
            });

            services.AddSingleton<IRelayService, ImmichRelay>();

            return services;
        }
    }
}
