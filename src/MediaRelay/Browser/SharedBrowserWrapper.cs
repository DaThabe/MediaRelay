namespace MediaRelay.Browser;


internal sealed class SharedBrowserWrapper(IBrowser inner) : IBrowser
{
    public string Version => inner.Version;

    public Task<IBrowserContext> NewContextAsync(BrowserNewContextOptions? options = null)
        => inner.NewContextAsync(options);

    public ValueTask DisposeAsync()
       => ValueTask.CompletedTask;
}