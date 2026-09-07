using MediaRelay.Console.Input;
using MediaRelay.Console.Logging;
using Microsoft.Extensions.Logging;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

public static class DependencyInjectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMediaRelayConsole()
        {
            services.AddHostedService<InputListenBackgroundService>();
            return services;
        }
    }

    extension(ILoggingBuilder builder)
    {
        public ILoggingBuilder AddMediaRelayConsole()
        {
            builder.Services.AddSingleton<ILoggerProvider, SpectreConsoleLoggerProvider>();
            return builder;
        }
    }
}
