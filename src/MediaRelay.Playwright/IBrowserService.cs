using Microsoft.Extensions.Options;
using Microsoft.Playwright;

namespace MediaRelay.Playwright;


public interface IBrowserService
{
    ValueTask<IBrowser> GetSharedAsync();
    ValueTask<IBrowser> LaunchAsync(BrowserTypeLaunchOptions? options = null);
}

internal sealed class ChromiumBrowserService(
    IPlaywrightService playwrightService,
    IOptions<BrowserTypeLaunchOptions> defaultLaunchOptions
    ) : IBrowserService, IAsyncDisposable
{
    private SharedBrowserWrapper? _sharedBrowser;
    private readonly SemaphoreSlim _sharedBrowserLock = new(1, 1);



    public async ValueTask<IBrowser> LaunchAsync(BrowserTypeLaunchOptions? options = null)
    {
        var playwright = await playwrightService.GetPlaywrightAsync();
        return await playwright.Chromium.LaunchAsync(options);
    }

    public async ValueTask<IBrowser> GetSharedAsync()
    {
        if (_sharedBrowser is not null) return _sharedBrowser;

        await _sharedBrowserLock.WaitAsync();
        try
        {
            if (_sharedBrowser is not null) return _sharedBrowser;

            var playwright = await playwrightService.GetPlaywrightAsync();
            var browser = await playwright.Chromium.LaunchAsync(defaultLaunchOptions.Value);

            return _sharedBrowser = new SharedBrowserWrapper(browser);
        }
        finally
        {
            _sharedBrowserLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_sharedBrowser is not null) await _sharedBrowser.DisposeAsync();
        _sharedBrowserLock.Dispose();
    }




    private sealed class SharedBrowserWrapper(IBrowser inner) : IBrowser
    {
        public IBrowserType BrowserType => inner.BrowserType;
        public IReadOnlyList<IBrowserContext> Contexts => inner.Contexts;
        public bool IsConnected => inner.IsConnected;
        public string Version => inner.Version;


        public event EventHandler<IBrowserContext> Context
        {
            add => inner.Context += value;
            remove => inner.Context -= value;
        }
        public event EventHandler<IBrowser> Disconnected
        {
            add => inner.Disconnected += value;
            remove => inner.Disconnected -= value;
        }

        public Task<BrowserBindResult> BindAsync(string title, BrowserBindOptions? options = null) => inner.BindAsync(title, options);
        public Task UnbindAsync() => inner.UnbindAsync();

        public Task<ICDPSession> NewBrowserCDPSessionAsync() => inner.NewBrowserCDPSessionAsync();
        public Task<IBrowserContext> NewContextAsync(BrowserNewContextOptions? options = null) => inner.NewContextAsync(options);
        public Task<IPage> NewPageAsync(BrowserNewPageOptions? options = null) => inner.NewPageAsync(options);


        public Task CloseAsync(BrowserCloseOptions? options = null) => Task.CompletedTask;
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}


