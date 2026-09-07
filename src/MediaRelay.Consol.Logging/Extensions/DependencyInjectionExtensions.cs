using MediaRelay.Console.Logging;
using Microsoft.Extensions.Logging;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

public static class DependencyInjectionExtensions
{
    extension(ILoggingBuilder builder)
    {
        public ILoggingBuilder AddMediaRelayConsole(LoggerStyle style = LoggerStyle.Default)
        {
            builder.Services.AddSingleton<ILoggerProvider>(LoggerProvider.FromStyle(style));
            return builder;
        }
    }
}
