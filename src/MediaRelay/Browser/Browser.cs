namespace MediaRelay.Browser;


internal sealed class Browser(Microsoft.Playwright.IBrowser browser) : IBrowser
{
    public string Version => browser.Version;

    public async Task<IBrowserContext> NewContextAsync(BrowserNewContextOptions? options = null)
    {
        var context = await browser.NewContextAsync(Parse(options));
        return new BrowserContext(context);
    }

    public ValueTask DisposeAsync()
    {
        return browser.DisposeAsync();
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