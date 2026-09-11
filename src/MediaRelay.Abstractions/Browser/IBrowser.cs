namespace MediaRelay.Browser;

public interface IBrowser : IAsyncDisposable
{
    string Version { get; }
    ValueTask<IBrowserContext> NewContextAsync(BrowserNewContextOptions? options = null);
}