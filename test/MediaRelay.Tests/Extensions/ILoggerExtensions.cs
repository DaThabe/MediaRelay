using Microsoft.Extensions.DependencyInjection;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.Logging;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class ILoggerExtensions
{
    extension<T>(ILogger<T>)
    {
        public static ILogger<T> Create() => _factory.CreateLogger<T>();
    }

    private static readonly ILoggerFactory _factory = LoggerFactory.Create(x => x.AddEmojiConsole());
}