using MediaRelay.Console;
using MediaRelay.Console.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spectre.Console;

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
                services.AddCustomConsoleLoggerProvider();
            });
        }
    }

    extension(ILoggingBuilder builder)
    {
        public ILoggingBuilder AddCustomConsole()
        {
            builder.Services.AddCustomConsoleLoggerProvider();
            return builder;
        }
    }

    extension(IServiceCollection services)
    {
        public IServiceCollection AddConsole()
        {
            services.AddHostedService<ConsoleInputUrlBackgroundService>();
            return services;
        }

        private IServiceCollection AddCustomConsoleLoggerProvider()
        {
            services.AddSingleton(_ => AnsiConsole.Create(new AnsiConsoleSettings()));
            services.TryAddSingleEnumerable<ILoggerProvider, ConsoleLoggerProvider>();

            return services;
        }
    }
}
