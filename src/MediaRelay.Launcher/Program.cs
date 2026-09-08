using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


await Host.CreateDefaultBuilder()
    .UseEnvironment(Environments.Development)
    .ConfigureLogging(builder => builder
        .ClearProviders()
        .AddEmojiDebug()
        .AddCustomConsole()
    )
    .ConfigureServices(services => services
        // Sources
        .AddPixiv()
        .AddTwitter()

        // Destinations
        .AddImmich()

        // Core
        .AddMediaRelay()
        .AddConsole()
    )
    .UseDevelopmentSecrets()
    .RunConsoleAsync();

