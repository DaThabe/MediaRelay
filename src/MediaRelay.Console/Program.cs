using MediaRelay.Immich;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


await Host.CreateDefaultBuilder()
    .ConfigureServices(x => x
        //Module
        .AddPlaywright()
        .AddHttpClient()
        .AddStorage()

        // Sources
        .AddPixivSource()

        // Destinations
        .AddImmichDestination()

        // Console
        .AddMediaRelay()
        .AddMediaRelayConsole()
        )
    .RunConsoleAsync();