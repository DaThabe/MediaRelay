using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

Console.Title = $"MediaRelay v2026.09.13";

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