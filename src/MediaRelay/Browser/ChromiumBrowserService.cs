using MediaRelay.Browser.Shared;
using MediaRelay.Playwright;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MediaRelay.Browser;


internal sealed class ChromiumBrowserService(
    IPlaywrightService playwrightService,
    IOptions<BrowserOptions> browserOptions,
    ILogger<ChromiumBrowserService> logger
    ) : IBrowserService, IAsyncDisposable
{
    private bool _disposed;
    private SharedBrowserWrapper? _sharedBrowser;
    private SharedBrowserContextWrapper? _sharedBrowserContext;
    private readonly SemaphoreSlim _sharedBrowserLock = new(1, 1);
    private readonly SemaphoreSlim _sharedBrowserContextLock = new(1, 1);


    public async ValueTask<IBrowser> GetSharedAsync()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_sharedBrowser is not null) return _sharedBrowser;


        await _sharedBrowserLock.WaitAsync();
        try
        {
            if (_sharedBrowser is not null) return _sharedBrowser;
            var playwright = await playwrightService.GetPlaywrightAsync();

            // 启动
            logger.LogInformation("正在启动共享浏览器");
            var browser = await playwright.Chromium.LaunchAsync(Parse(browserOptions.Value.Launch));
            _sharedBrowser = new SharedBrowserWrapper(new Browser(browser, logger));

            // 完成
            using var __ = logger.BeginScope("BrowserVersion", _sharedBrowser.Version);
            logger.LogInformation("共享浏览器已启动");

            return _sharedBrowser;
        }
        finally
        {
            _sharedBrowserLock.Release();
        }
    }
    public async ValueTask<IBrowserContext> GetSharedContextAsync()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_sharedBrowserContext is not null) return _sharedBrowserContext;

        await _sharedBrowserContextLock.WaitAsync();
        try
        {
            if (_sharedBrowserContext is not null) return _sharedBrowserContext;
            var sharedBrowser = await GetSharedAsync();

            // 启动
            logger.LogInformation("正在创建浏览器共享上下文");
            var browserContext = await sharedBrowser.NewContextAsync(browserOptions.Value.NewContext);
            logger.LogInformation("浏览器共享上下文已创建");

            return _sharedBrowserContext = new SharedBrowserContextWrapper(browserContext);
        }
        finally
        {
            _sharedBrowserContextLock.Release();
        }
    }

    public ValueTask<IBrowser> LaunchDefaultAsync()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        return LaunchAsync(browserOptions.Value.Launch);
    }
    public async ValueTask<IBrowser> LaunchAsync(BrowserLaunchOptions? options = null)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var playwright = await playwrightService.GetPlaywrightAsync();
        var browser = await playwright.Chromium.LaunchAsync(Parse(options));
        logger.LogInformation("正在启动浏览器");

        return new Browser(browser, logger);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        if (_sharedBrowser is not null) await _sharedBrowser.DisposeAsync();
        _sharedBrowserLock.Dispose();

        _disposed = true;

        logger.LogDebug("浏览器业务已释放");
    }


    private static Microsoft.Playwright.BrowserTypeLaunchOptions? Parse(BrowserLaunchOptions? options)
    {
        if (options is null) return null;

        return new()
        {
            Headless = options.Headless,
            Timeout = (float)options.Timeout.TotalMilliseconds,
            ExecutablePath = options.ExecutablePath,
            DownloadsPath = options.DownloadsPath,
            Args = options.Args,
        };
    }
}