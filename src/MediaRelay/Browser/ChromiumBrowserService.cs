using MediaRelay.Browser.Shared;
using MediaRelay.Playwright;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MediaRelay.Browser;


internal sealed class ChromiumBrowserService(
    IPlaywrightService playwrightService,
    IOptions<BrowserLaunchOptions> launchOptions,
    IOptions<BrowserNewContextOptions> newContextOptions,
    ILogger<ChromiumBrowserService> logger
    ) : IBrowserService, IAsyncDisposable
{
    private SharedBrowserWrapper? _sharedBrowser;
    private SharedBrowserContextWrapper? _sharedBrowserContext;
    private readonly SemaphoreSlim _sharedBrowserLock = new(1, 1);
    private readonly SemaphoreSlim _sharedBrowserContextLock = new(1, 1);


    public async ValueTask<IBrowser> GetSharedAsync()
    {
        if (_sharedBrowser is not null) return _sharedBrowser;

        await _sharedBrowserLock.WaitAsync();
        try
        {
            if (_sharedBrowser is not null) return _sharedBrowser;
            var playwright = await playwrightService.GetPlaywrightAsync();

            // 启动
            logger.LogInformation("正在启动共享浏览器");
            var browser = await playwright.Chromium.LaunchAsync(Parse(launchOptions.Value));
            _sharedBrowser = new SharedBrowserWrapper(new Browser(browser));

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
        if (_sharedBrowserContext is not null) return _sharedBrowserContext;

        await _sharedBrowserContextLock.WaitAsync();
        try
        {
            if (_sharedBrowserContext is not null) return _sharedBrowserContext;
            var sharedBrowser = await GetSharedAsync();

            // 启动
            logger.LogInformation("正在创建浏览器共享上下文");
            var browserContext = await sharedBrowser.NewContextAsync(newContextOptions.Value);
            _sharedBrowserContext = new SharedBrowserContextWrapper(browserContext);
            logger.LogInformation("浏览器共享上下文已创建");

            return _sharedBrowserContext;
        }
        finally
        {
            _sharedBrowserContextLock.Release();
        }
    }



    public ValueTask<IBrowser> LaunchDefaultAsync()
    {
        return LaunchAsync(launchOptions.Value);
    }
    public async ValueTask<IBrowser> LaunchAsync(BrowserLaunchOptions? options = null)
    {
        var playwright = await playwrightService.GetPlaywrightAsync();
        var broser = await playwright.Chromium.LaunchAsync(Parse(options));

        return new Browser(broser);
    }


    public async ValueTask DisposeAsync()
    {
        if (_sharedBrowser is not null) await _sharedBrowser.DisposeAsync();
        _sharedBrowserLock.Dispose();
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