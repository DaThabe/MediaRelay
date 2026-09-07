using MediaRelay.Console.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


await Host.CreateDefaultBuilder()
    .ConfigureLogging(builder => builder
        .ClearProviders()
        .AddEmojiDebug()
        .AddMediaRelayConsole(LoggerStyle.Default)
    )
    .ConfigureServices(services => services
        // Sources
        .AddPixivSource()
        .AddTwitterSource()

        // Destinations
        .AddImmichDestination()

        // Core
        .AddMediaRelay()
        .AddMediaRelayConsole()
        )
    .RunConsoleAsync();

