using MediaRelay.Http;
using Microsoft.Extensions.Logging;

namespace MediaRelay.Browser;


internal sealed class BrowserContext(Microsoft.Playwright.IBrowserContext context, ILogger logger) : IBrowserContext
{
    public async ValueTask<IPage> NewPageAsync()
    {
        var page = await context.NewPageAsync();
        logger.LogDebug("新建页面");

        return new Page(page, logger);
    }
    public async ValueTask AddCookiesAsync(IEnumerable<HttpCookieOptions> cookieOptions)
    {
        var cookies = cookieOptions.Select(Parse);
        await context.AddCookiesAsync(cookies);

        var cookieString = string.Join(',', cookies.Select(x => $"{x.Domain}/{x.Name}"));
        using var _ = logger.BeginScope("Cookies", $"[ {cookieString} ]");
        logger.LogDebug("Cookie已添加");
    }
    public async ValueTask DisposeAsync()
    {
        await context.DisposeAsync();
        logger.LogDebug("浏览器上下文已释放");
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
