using Microsoft.Extensions.Logging;

namespace MediaRelay.Browser;


internal sealed class Browser(Microsoft.Playwright.IBrowser browser, ILogger logger) : IBrowser
{
    public string Version => browser.Version;

    public async Task<IBrowserContext> NewContextAsync(BrowserNewContextOptions? options = null)
    {
        var context = await browser.NewContextAsync(Parse(options));
        logger.LogDebug("已创建浏览器上下文");

        return new BrowserContext(context, logger);
    }

    public async ValueTask DisposeAsync()
    {
        await browser.DisposeAsync();
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
        if (size is null ) return null;
        return new() { Width = size.Value.Width, Height = size.Value.Height };
    }
}