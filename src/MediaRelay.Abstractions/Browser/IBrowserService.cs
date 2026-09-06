namespace MediaRelay.Browser;


public interface IBrowserService
{
    ValueTask<IBrowser> GetSharedAsync();

    ValueTask<IBrowser> LaunchDefaultAsync();
    ValueTask<IBrowser> LaunchAsync(BrowserLaunchOptions? options = null);
}
