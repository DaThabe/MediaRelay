using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

Console.Title = GetVersionName();

await Host.CreateDefaultBuilder(args)
    .ConfigureLogging(builder => builder
        .ClearProviders()
        .AddEmojiDebug()
        .AddCustomConsole()
    )
    .ConfigureServices(services => services
        .AddMediaRelay()
        .AddUrl()
        // Modules
        .AddPixiv()
        .AddTwitter()
        .AddHeyBox()
        .AddImmich()
        .AddConsole()
        .AddClipboard()
    )
    .RunConsoleAsync();



static string GetVersionName()
{
    var version = typeof(Program).Assembly
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
        ?.InformationalVersion ?? "unknown";

    return $"MediaRelay v{version}";
}