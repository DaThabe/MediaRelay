using MediaRelay.Browser.Shared;
using MediaRelay.Serialization;
using System.Diagnostics.CodeAnalysis;

namespace MediaRelay.Browser;


internal sealed class PageSessionFactory(IBrowserService browserService) : IPageSessionFactory
{
    public async ValueTask<IPageSession> CreateAsync(BrowserNewContextOptions? options = null)
    {
        // Browser
        var browser = await browserService.GetSharedAsync();

        // Context
        var context = await browser.NewContextAsync(options);

        // Page
        var page = await context.NewPageAsync();


        return new PageSession(context, page);
    }
}

file sealed class PageSession(IBrowserContext context, IPage page) : IPageSession
{
    private bool _disposed;


    bool IPage.IsClosed => page.IsClosed;
    public IBrowserContext Context { get; } = new SharedBrowserContext(context);


    public ValueTask<T> EvaluateAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties)] T>(string expression, object? arg, CancellationToken cancellationToken)
        => page.EvaluateAsync<T>(expression, arg, cancellationToken);
    public ValueTask<T> EvaluateAsync<T>(string expression, object? arg, ISerializer<T> serializer, CancellationToken cancellationToken) where T : notnull
        => page.EvaluateAsync(expression, arg, serializer, cancellationToken);
    public ValueTask GotoAsync(string url, PageGotoOptions? options, CancellationToken cancellationToken)
        => page.GotoAsync(url, options, cancellationToken);
    public ValueTask CloseAsync()
        => page.DisposeAsync();


    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;

        await page.DisposeAsync();
        await context.DisposeAsync();
    }
}