using MediaRelay.Http;

namespace MediaRelay.Browser;


internal sealed class BrowserContextWapper(Microsoft.Playwright.IBrowserContext context) : IBrowserContext
{
    public async Task<IPage> NewPageAsync()
    {
        var page = await context.NewPageAsync();
        return new PageWapper(page);
    }
    public ValueTask DisposeAsync()
    {
        return context.DisposeAsync();
    }

    public Task AddCookiesAsync(IEnumerable<HttpCookieOptions> cookieOptions)
    {
        var cookies = cookieOptions.Select(Parse);
        return context.AddCookiesAsync(cookies);
    }


    private static Microsoft.Playwright.Cookie Parse(HttpCookieOptions options)
    {
        return new()
        {
            Name = options.Name,
            Domain = options.Domain,
            Value = options.Value,
            Path = options.Path,

            Expires = options.Expires.HasValue ? options.Expires.Value.ToUnixTimeSeconds() : null,
            HttpOnly = options.HttpOnly,
            Secure = options.Secure,

            Url = options.Url,
            PartitionKey = options.PartitionKey,
            SameSite = Parse(options.SameSite)
        };
    }

    private static Microsoft.Playwright.SameSiteAttribute? Parse(HttpCookieSameSite? options)
    {
        if (options is null) return null;

        return options switch
        {
            HttpCookieSameSite.Strict => Microsoft.Playwright.SameSiteAttribute.Strict,
            HttpCookieSameSite.Lax => Microsoft.Playwright.SameSiteAttribute.Lax,
            HttpCookieSameSite.None => Microsoft.Playwright.SameSiteAttribute.None,
            _ => null
        };
    }
}
