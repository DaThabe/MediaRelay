namespace MediaRelay.Browser.Shared;


internal sealed class SharedBrowserWrapper(IBrowser inner) : IBrowser
{
    public string Version => inner.Version;

    public ValueTask<IBrowserContext> NewContextAsync(BrowserNewContextOptions? options = null)
        => inner.NewContextAsync(options);
    public ValueTask DisposeAsync()
       => ValueTask.CompletedTask;
}
