using MediaRelay.Clipboard;
using Microsoft.Extensions.Configuration;
using TextCopy;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddClipboard()
        {
            services.AddOptions<ClipboardOptions>()
               .Configure<IConfiguration>((options, configuration) => configuration
                   .GetSection(ClipboardOptions.SectionPath)
                   .Bind(options));

            services.InjectClipboard();
            services.AddHostedService<ClipboardUrlBackgroundService>();

            return services;
        }
    }
}
