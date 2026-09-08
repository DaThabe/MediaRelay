using MediaRelay.Console;
using MediaRelay.Console.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

public static class DependencyInjectionExtensions
{
    extension(IHostBuilder host)
    {
        public IHostBuilder UseConsole()
        {
            return host.ConfigureServices((_, services) =>
            {
                services.AddConsole();
                services.TryAddInstanceEnumerable<ILoggerProvider>(ConsoleLoggerProvider.Instance);
            });
        }
    }

    extension(IServiceCollection services)
    {
        public IServiceCollection AddConsole()
        {
            services.AddHostedService<ConsoleInputUrlBackgroundService>();
            return services;
        }
    }

    extension(ILoggingBuilder builder)
    {
        public ILoggingBuilder AddCustomConsole()
        {
            builder.Services.TryAddInstanceEnumerable<ILoggerProvider>(ConsoleLoggerProvider.Instance);
            return builder;
        }
    }
}
