using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MediaRelay.GUI.ViewModels;
using MediaRelay.GUI.Views;

namespace MediaRelay.GUI;


#if DEBUG
public class App : Application
{
#else
public partial class App(IHostBuilder builder) : Application
{
    private readonly IHost _host = builder.Build();
#endif

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
#if RELEASE
        _host.Start();
#endif

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel() { MainView = new MainViewModel() }
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
