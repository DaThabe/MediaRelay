using MediaRelay.Http;

namespace MediaRelay.Browser;

public interface IBrowserContext : IAsyncDisposable
{
    ValueTask<IPage> NewPageAsync();
    ValueTask AddCookiesAsync(IEnumerable<HttpCookieOptions> cookieOptions);
}