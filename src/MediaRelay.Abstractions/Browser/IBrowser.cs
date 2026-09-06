namespace MediaRelay.Browser;

public interface IBrowser : IAsyncDisposable
{
    string Version { get; }
    Task<IBrowserContext> NewContextAsync(BrowserNewContextOptions? options = null);
}