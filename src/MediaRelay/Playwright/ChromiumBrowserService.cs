using MediaRelay.Browser;
using MediaRelay.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MediaRelay.Playwright;


internal sealed class ChromiumBrowserService(
    IPlaywrightService playwrightService,
    IOptions<BrowserLaunchOptions> options,
    ILogger<ChromiumBrowserService> logger
    ) : IBrowserService, IAsyncDisposable
{
    private SharedBrowserWrapper? _sharedBrowser;
    private readonly SemaphoreSlim _sharedBrowserLock = new(1, 1);


    public ValueTask<IBrowser> LaunchDefaultAsync()
    {
        return LaunchAsync(options.Value);
    }
    public async ValueTask<IBrowser> LaunchAsync(BrowserLaunchOptions? options = null)
    {
        var playwright = await playwrightService.GetPlaywrightAsync();
        var broser = await playwright.Chromium.LaunchAsync(Parse(options));

        return new BrowserWapper(broser);
    }

    public async ValueTask<IBrowser> GetSharedAsync()
    {
        if (_sharedBrowser is not null) return _sharedBrowser;

        await _sharedBrowserLock.WaitAsync();
        try
        {
            if (_sharedBrowser is not null) return _sharedBrowser;

            var playwright = await playwrightService.GetPlaywrightAsync();
            logger.LogInformation("正在启动浏览器");

            // 启动
            var launchOptions = Parse(options.Value);
            var browser = await playwright.Chromium.LaunchAsync(launchOptions);
            _sharedBrowser = new SharedBrowserWrapper(new BrowserWapper(browser));

            // 完成
            using var __ = logger.BeginScope("BrowserVersion", _sharedBrowser.Version);
            logger.LogInformation("浏览器已启动");
            return _sharedBrowser;
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