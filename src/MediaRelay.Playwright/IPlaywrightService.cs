using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

namespace MediaRelay.Playwright;


public interface IPlaywrightService
{
    ValueTask<IPlaywright> GetPlaywrightAsync();
}

internal sealed partial class PlaywrightService(
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

            LogInit();
            _playwright ??= await Microsoft.Playwright.Playwright.CreateAsync();
            LogComplete();

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



    [LoggerMessage(Level = LogLevel.Information, Message = "开始初始化")]
    private partial void LogInit();

    [LoggerMessage(Level = LogLevel.Information, Message = "初始化完成")]
    private partial void LogComplete();
}