using MediaRelay.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MediaRelay.Console.Input;

public interface IUrlPersistentQueue
{
    int Count { get; }

    ValueTask WriteAsync(Uri url, CancellationToken cancellationToken);
    ValueTask<Uri> ReadWaitAsync(CancellationToken cancellationToken);
}

internal sealed class UrlPersistentQueue : IUrlPersistentQueue, IDisposable
{
    private bool _disposed;
    private readonly Queue<Uri> _values = new();
    private readonly SemaphoreSlim _lock = new(1, 1);
    private TaskCompletionSource? writeTcs;
    private readonly Func<CancellationToken, Task> _saveHandler;


    public int Count => _values.Count;


    public UrlPersistentQueue(IOptions<ConsoleOptions> options, ILogger<ConsoleOptions> logger)
    {
        // Load
        if (File.Exists(options.Value.UnprocessedInputFile))
        {
            foreach (var i in File.ReadAllLines(options.Value.UnprocessedInputFile))
            {
                if (!Uri.TryCreate(i, UriKind.Absolute, out var uri)) continue;
                _values.Enqueue(uri);
            }
        }

        // Saver
        _saveHandler = ct =>
        {
            var lines = _values.ToArray().Select(x => x.ToString());
            var folder = Path.GetDirectoryName(options.Value.UnprocessedInputFile);
            if (!string.IsNullOrWhiteSpace(folder)) Directory.CreateDirectory(folder);

            return File.WriteAllLinesAsync(options.Value.UnprocessedInputFile, lines, ct);
        };
    }

    public async ValueTask WriteAsync(Uri url, CancellationToken cancellationToken)
    {
        using var _ = await _lock.WaitScopeAsync(cancellationToken);

        _values.Enqueue(url);
        await _saveHandler(cancellationToken);

        writeTcs?.TrySetResult();
    }
    public async ValueTask<Uri> ReadWaitAsync(CancellationToken cancellationToken)
    {
        using (await _lock.WaitScopeAsync(cancellationToken))
        {
            if (_values.Count > 0)
            {
                var value = _values.Dequeue();
                await _saveHandler(cancellationToken);
                return value;
            }
            writeTcs = new TaskCompletionSource();
        }

        // 等待写入
        await using var registration = cancellationToken.Register(() =>
            writeTcs?.TrySetCanceled(cancellationToken));
        await writeTcs.Task;

        // 弹出数据
        using (await _lock.WaitScopeAsync(cancellationToken))
        {
            var value = _values.Dequeue();
            await _saveHandler(cancellationToken);
            return value;
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        writeTcs?.TrySetCanceled();
        _lock.Dispose();
    }
}