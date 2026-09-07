namespace MediaRelay.Browser;


public interface IBrowserService
{
    ValueTask<IBrowser> GetSharedAsync();
    ValueTask<IBrowserContext> GetSharedContextAsync();


    ValueTask<IBrowser> LaunchDefaultAsync();
    ValueTask<IBrowser> LaunchAsync(BrowserLaunchOptions? options = null);
}