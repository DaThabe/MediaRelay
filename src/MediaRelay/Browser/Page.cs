using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace MediaRelay.Browser;


internal sealed class Page(Microsoft.Playwright.IPage page, ILogger logger) : IPage
{
    public Task<T> EvaluateAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties)] T>(string expression, object? arg = default)
    {
        return page.EvaluateAsync<T>(expression, arg);
    }

    public async Task GotoAsync(string url, PageGotoOptions? options = null, CancellationToken cancellationToken = default)
    {
        await using var registration = cancellationToken.Register(async () =>
        {
            await page.CloseAsync();
            logger.LogInformation("页面已取消");
        });

        try
        {
            await page.GotoAsync(url, Parse(options));
            logger.LogInformation("页面已跳转");
            return;
        }
        catch (Exception)
        {
            await page.CloseAsync();
            throw;
        }
    }

    public ValueTask DisposeAsync()
    {
        return page.DisposeAsync();
    }


    private static Microsoft.Playwright.PageGotoOptions? Parse(PageGotoOptions? options)
    {
        if (options is null) return null;

        return new Microsoft.Playwright.PageGotoOptions()
        {
            Timeout = (float?)options.Timeout?.TotalMilliseconds,
            WaitUntil = Parse(options.WaitUntil),
            Referer = options.Referer
        };
    }
    private static Microsoft.Playwright.WaitUntilState? Parse(WaitUntilState? state)
    {
        return state switch
        {
            WaitUntilState.DOMContentLoaded => Microsoft.Playwright.WaitUntilState.DOMContentLoaded,
            WaitUntilState.Commit => Microsoft.Playwright.WaitUntilState.Commit,
            WaitUntilState.NetworkIdle => Microsoft.Playwright.WaitUntilState.NetworkIdle,
            WaitUntilState.Load => Microsoft.Playwright.WaitUntilState.Load,
            _ => null
        };
    }
}