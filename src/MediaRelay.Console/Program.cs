using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


await Host.CreateDefaultBuilder()
    .ConfigureLogging(builder => builder
        .ClearProviders()
        .AddMediaRelayDebug()
        .AddMediaRelayConsole()
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