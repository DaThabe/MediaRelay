using MediaRelay.Http;

namespace MediaRelay.Browser.Shared;

internal sealed class SharedBrowserContextWrapper(IBrowserContext inner) : IBrowserContext
{
    public ValueTask<IPage> NewPageAsync()
        => inner.NewPageAsync();
    public ValueTask AddCookiesAsync(IEnumerable<HttpCookieOptions> cookieOptions)
        => inner.AddCookiesAsync(cookieOptions);
    public ValueTask DisposeAsync()
       => ValueTask.CompletedTask;
}