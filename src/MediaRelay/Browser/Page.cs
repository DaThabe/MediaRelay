using System.Diagnostics.CodeAnalysis;

namespace MediaRelay.Browser;


internal sealed class Page(Microsoft.Playwright.IPage page) : IPage
{
    public Task<T> EvaluateAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties)] T>(string expression, object? arg = default)
    {
        return page.EvaluateAsync<T>(expression, arg);
    }

    public Task GotoAsync(string url, PageGotoOptions? options = null)
    {
        return page.GotoAsync(url, Parse(options));
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