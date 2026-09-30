using Avalonia;
using MediaRelay.GUI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MediaRelay.Launcher.Desktop;

static class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    public static async Task Main(string[] args)
    {
        var app = Host.CreateDefaultBuilder(args)
            .ConfigureLogging(builder => builder
                //.ClearProviders()
                .AddEmojiDebug()
            )
            .ConfigureServices(services => services
                .AddMediaRelay()
                .AddUrl()
                // Modules
                .AddPixiv()
                .AddTwitter()
                .AddHeyBox()
                .AddImmich()
                .AddClipboard()
                .AddDesktop()
                // Launcher
                .AddSingleton(new CommandLineArgs(args))
                .AddSingleton(BuildAvaloniaApp())
                .AddHostedService<AvaloniaDesktopLifetime>()
            )
            .Build();

        await app.StartAsync();
        await app.WaitForShutdownAsync();
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();


    private sealed record class CommandLineArgs(string[] Args);
    private sealed class AvaloniaDesktopLifetime(IHostApplicationLifetime hostApplicationLifetime, AppBuilder builder, CommandLineArgs commandLineArgs) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!OperatingSystem.IsWindows())
            {
                builder.StartWithClassicDesktopLifetime(commandLineArgs.Args);
            }
            else
            {
                var tcs = new TaskCompletionSource();
                var thread = new Thread(() =>
                {
                    try
                    {
                        builder.StartWithClassicDesktopLifetime(commandLineArgs.Args);
                        tcs.SetResult();
                    }
                    catch (Exception ex)
                    {
                        tcs.SetException(ex);
                    }
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();

                await tcs.Task;
            }

            hostApplicationLifetime.StopApplication();
        }
    }
}