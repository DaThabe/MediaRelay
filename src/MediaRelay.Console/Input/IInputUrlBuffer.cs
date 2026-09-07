using MediaRelay.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Channels;

namespace MediaRelay.Console.Input;

public interface IInputUrlBuffer
{
    int Count { get; }

    ValueTask WriteAsync(Uri url, CancellationToken cancellationToken);
    ValueTask<Uri> ReadAsync(CancellationToken cancellationToken);
    ValueTask<Uri> WaitPeepAsync(CancellationToken cancellationToken);
    ValueTask<Uri> WaitReadAsync(CancellationToken cancellationToken);
}

internal sealed class InputUrlBuffer(
        IOptions<ConsoleOptions> options,
        ILogger<ConsoleOptions> logger
    ) : IInputUrlBuffer, IAsyncDisposable
{
    private bool _inited;
    private bool _disposed;
    private Channel<Uri> _values = Channel.CreateUnbounded<Uri>();
    private readonly SemaphoreSlim _lock = new(1, 1);


    public int Count => _values.Reader.Count;




    public async ValueTask WriteAsync(Uri url, CancellationToken cancellationToken)
    {
        await InitAsync(cancellationToken);
        await _values.Writer.WriteAsync(url, cancellationToken);
    }

    public async ValueTask<Uri> ReadAsync(CancellationToken cancellationToken)
    {
        await InitAsync(cancellationToken);
        return await _values.Reader.ReadAsync(cancellationToken);
    }
    public async ValueTask<Uri> WaitReadAsync(CancellationToken cancellationToken)
    {
        await InitAsync(cancellationToken);

        if (!await _values.Reader.WaitToReadAsync(cancellationToken))
            throw new InvalidOperationException("没有输入记录了");

        return await _values.Reader.ReadAsync(cancellationToken);
    }
    public async ValueTask<Uri> WaitPeepAsync(CancellationToken cancellationToken)
    {
        await InitAsync(cancellationToken);

        if (await _values.Reader.WaitToReadAsync(cancellationToken) && _values.Reader.TryPeek(out var uri))
            return uri;

        throw new InvalidOperationException("没有输入记录了");
    }


    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        try
        {
            List<Uri> urls = [];
            while (_values.Reader.TryRead(out var url)) urls.Add(url);

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            await SaveAsync([.. urls], cts.Token);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "输入记录释放时发生异常");
        }

        _disposed = true;
        GC.SuppressFinalize(this);
    }


    private async ValueTask InitAsync(CancellationToken cancellationToken)
    {
        if (_inited) return;

        await LoadAsync(cancellationToken);
        _inited = true;
    }

    private async ValueTask LoadAsync(CancellationToken cancellationToken)
    {
        using var _ = await _lock.WaitScopeAsync(cancellationToken);

        try
        {
            if (!File.Exists(options.Value.UnprocessedInputFile)) return;
            var lines = await File.ReadAllLinesAsync(options.Value.UnprocessedInputFile, cancellationToken);

            foreach (var line in lines)
            {
                if (!Uri.TryCreate(line, UriKind.Absolute, out var url)) continue;
                await _values.Writer.WriteAsync(url, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "输入记录加载失败");
        }
    }
    private async ValueTask SaveAsync(Uri[] urls, CancellationToken cancellationToken)
    {
        using var _ = await _lock.WaitScopeAsync(cancellationToken);

        var lines = urls.Select(x => x.ToString()).ToArray();
        using var __ = logger.BeginScope("Urls", $"[{string.Join(',', lines)}]");

        try
        {
            await File.WriteAllLinesAsync(options.Value.UnprocessedInputFile, lines, cancellationToken);
            logger.LogInformation("输入记录已储存");
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "输入记录储存失败");
        }
    }


}