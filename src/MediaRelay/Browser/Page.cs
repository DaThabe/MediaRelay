using MediaRelay.Serializer;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace MediaRelay.Browser;


internal sealed class Page(Microsoft.Playwright.IPage page, ILogger logger) : IPage
{
    private bool _disposed;

    public bool IsClosed => page.IsClosed;


    public async ValueTask<T> EvaluateAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties)] T>(
        string expression, object? arg = default, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(page.IsClosed, this);

        try
        {
            return await page.EvaluateAsync<T>(expression, arg)
                .WaitAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("页面脚本执行已取消");
            await DisposeAsync();
            throw;
        }
        catch (Exception)
        {
            await DisposeAsync();
            throw;
        }
    }

    public async ValueTask<JsonElement?> EvaluateAsync(
        string expression, object? arg = null, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(page.IsClosed, this);

        try
        {
            return await page.EvaluateAsync(expression, arg)
                .WaitAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("页面脚本执行已取消");
            await DisposeAsync();
            throw;
        }
        catch (Exception)
        {
            await DisposeAsync();
            throw;
        }
    }

    public async ValueTask GotoAsync(
        string url, PageGotoOptions? options = null, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(page.IsClosed, this);

        try
        {
            await page.GotoAsync(url, Parse(options))
                .WaitAsync(cancellationToken);

            logger.LogInformation("页面已跳转");
            return;
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("页面已取消跳转");
            await DisposeAsync();
            throw;
        }
        catch (Exception)
        {
            await DisposeAsync();
            throw;
        }
    }

    public ValueTask CloseAsync()
    {
        var task = page.CloseAsync();
        return new ValueTask(task);
    }
    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;

        try
        {
            await page.DisposeAsync();
            logger.LogDebug("页面已释放");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "释放页面时出错");
        }
    }



    private static Microsoft.Playwright.PageGotoOptions? Parse(PageGotoOptions? options)
    {
        if (options is null) return null;

        return new Microsoft.Playwright.PageGotoOptions()
        {
            Timeout = (float?)options.Timeout?.TotalMilliseconds,
            WaitUntil = Parse(options.WaitUntil),
            Referer = options.Referer
        };
    }
    private static Microsoft.Playwright.WaitUntilState? Parse(WaitUntilState? state)
    {
        return state switch
        {
            WaitUntilState.DOMContentLoaded => Microsoft.Playwright.WaitUntilState.DOMContentLoaded,
            WaitUntilState.Commit => Microsoft.Playwright.WaitUntilState.Commit,
            WaitUntilState.NetworkIdle => Microsoft.Playwright.WaitUntilState.NetworkIdle,
            WaitUntilState.Load => Microsoft.Playwright.WaitUntilState.Load,
            _ => null
        };
    }


    public ValueTask<T> EvaluateAsync<T>(string expression, object? arg, ISerializer<T> serializer, CancellationToken cancellationToken = default) where T : notnull
    {
        throw new NotImplementedException();
    }
}