using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


await Host.CreateDefaultBuilder()
    .ConfigureLogging(builder =>
    {
        builder.ClearProviders();
        builder.AddDebug();
        builder.AddMediaRelayConsole();
    })
    .ConfigureServices(x => x
        //Module
        .AddPlaywright()
        .AddHttpClient()
        .AddStorage()

        // Sources
        .AddPixivSource()

        // Destinations
        .AddImmichDestination()

        // Core
        .AddMediaRelay()
        .AddMediaRelayConsole()
        )
    .RunConsoleAsync();