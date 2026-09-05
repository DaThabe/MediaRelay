using MediaRelay.Resources;
using MediaRelay.Storage;
using Microsoft.Extensions.Configuration;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

public static class DependencyInjectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddStorage()
        {
            services.AddOptions<StorageOptions>()
               .Configure<IConfiguration>((options, configuration) => configuration
                    .GetSection(StorageOptions.Name)
                    .Bind(options));

            services.AddSingleton<IHasher, SHA256Hasher>();

            services.AddSingleton<IStorage, Storage>();
            services.AddSingleton<IResourceStorage, ResourceStorage>();

            return services;
        }
    }
}
