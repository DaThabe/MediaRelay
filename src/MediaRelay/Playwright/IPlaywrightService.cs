using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

namespace MediaRelay.Playwright;


internal interface IPlaywrightService
{
    ValueTask<IPlaywright> GetPlaywrightAsync();
}

internal sealed class PlaywrightService(
        ILogger<PlaywrightService> logger
    ) : IPlaywrightService, IDisposable
{
    private bool _isDisposed;
    private IPlaywright? _playwright;
    private readonly SemaphoreSlim _lock = new(1, 1);


    public async ValueTask<IPlaywright> GetPlaywrightAsync()
    {
        if (_playwright is not null) return _playwright;

        await _lock.WaitAsync();
        try
        {
            if (_playwright is not null) return _playwright;

            logger.LogInformation("开始初始化");
            _playwright ??= await Microsoft.Playwright.Playwright.CreateAsync();
            logger.LogInformation("初始化完成");

            return _playwright;
        }
        finally
        {
            _lock.Release();
        }
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        _isDisposed = true;

        _playwright?.Dispose();
        _playwright = null;

        _lock.Dispose();
    }
}