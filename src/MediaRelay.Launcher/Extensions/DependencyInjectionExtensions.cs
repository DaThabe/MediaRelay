using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

public static class DependencyInjectionExtensions
{
    extension(IHostBuilder builder)
    {
        public IHostBuilder UseDevelopmentSecrets()
        {
            return builder.ConfigureAppConfiguration((context, builder) =>
            {
                if (!context.HostingEnvironment.IsDevelopment()) return;
                builder.AddUserSecrets<Program>(false, true);
            });
        }
    }
}
