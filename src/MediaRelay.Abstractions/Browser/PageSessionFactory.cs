namespace MediaRelay.Browser;


public interface IPageSessionFactory
{
    ValueTask<IPageSession> CreateAsync(BrowserNewContextOptions? options = null);
}

public interface IPageSession : IPage
{
    IBrowserContext Context { get; }
}