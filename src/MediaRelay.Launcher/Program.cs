using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

Console.Title = "MediaRelay v0.0.1";

await Host.CreateDefaultBuilder(args)
    .ConfigureLogging(builder => builder
        .ClearProviders()
        .AddEmojiDebug()
        .AddCustomConsole()
    )
    .ConfigureServices(services => services    
        .AddMediaRelay()
        // Modules
        .AddPixiv()
        .AddTwitter()
        .AddImmich()
        .AddConsole()
    )
    .UseDevelopmentSecrets()
    .RunConsoleAsync();

