using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

Console.Title = "MediaRelay v20260910.2057";


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
        .AddImmich()
        .AddConsole()
        .AddClipboard()
    )
    .RunConsoleAsync();