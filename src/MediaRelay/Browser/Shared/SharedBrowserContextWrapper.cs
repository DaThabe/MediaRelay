using MediaRelay.Http;

namespace MediaRelay.Browser.Shared;

internal sealed class SharedBrowserContextWrapper(IBrowserContext inner) : IBrowserContext
{
    public Task<IPage> NewPageAsync()
        => inner.NewPageAsync();
    public Task AddCookiesAsync(IEnumerable<HttpCookieOptions> cookieOptions)
        => inner.AddCookiesAsync(cookieOptions);
    public ValueTask DisposeAsync()
       => ValueTask.CompletedTask;
}