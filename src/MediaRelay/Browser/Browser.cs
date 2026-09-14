using Microsoft.Extensions.Logging;

namespace MediaRelay.Browser;


internal sealed class Browser(Microsoft.Playwright.IBrowser browser, ILogger logger) : IBrowser
{
    private bool _disposed;

    public string Version
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return browser.Version;
        }
    }

    public async ValueTask<IBrowserContext> NewContextAsync(BrowserNewContextOptions? options = null)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);


        var context = await browser.NewContextAsync(Parse(options));
        logger.LogDebug("已创建浏览器上下文");

        return new BrowserContext(context, logger);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        await browser.DisposeAsync();
        _disposed = true;

        logger.LogDebug("浏览器已释放");
    }


    private static Microsoft.Playwright.BrowserNewContextOptions? Parse(BrowserNewContextOptions? options)
    {
        if (options is null) return null;

        return new()
        {
            ViewportSize = Parse(options.ViewportSize)
        };
    }

    private static Microsoft.Playwright.ViewportSize? Parse(ViewportSize? size)
    {
        if (size is null) return null;
        return new() { Width = size.Value.Width, Height = size.Value.Height };
    }
}