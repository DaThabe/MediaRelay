using MediaRelay.Http;

namespace MediaRelay.Browser;

public interface IBrowserContext : IAsyncDisposable
{
    Task<IPage> NewPageAsync();
    Task AddCookiesAsync(IEnumerable<HttpCookieOptions> cookieOptions);
}