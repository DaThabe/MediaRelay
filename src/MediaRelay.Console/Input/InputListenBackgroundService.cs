using AsyncConsoleReader;
using MediaRelay.Source.Url;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;
using System.Threading.Channels;

namespace MediaRelay.Console.Input;


internal sealed class InputListenBackgroundService(
        IMediaRelay mediaRelay,
        IUrlSourceParserSelector urlSourceParserSelector,
        ILogger<InputListenBackgroundService> logger
    ) : BackgroundService, IAsyncDisposable
{
    private int _requestId;
    private Task? _consumerTask;
    private readonly Channel<Uri> _urlChannel = Channel.CreateBounded<Uri>(50);



    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumerTask = ConsumeAsync(stoppingToken);
        return InputAsync(stoppingToken);
    }


    public override void Dispose()
    {
        DisposeAsync().AsTask().GetAwaiter().GetResult();
    }
    public async ValueTask DisposeAsync()
    {
        _urlChannel.Writer.TryComplete();
        if (_consumerTask is not null) await _consumerTask;

        _urlChannel.Writer.TryComplete();
    }


    private Task ConsumeAsync(CancellationToken cancellationToken)
    {
        return Task.Run(async () =>
        {
            logger.LogInformation("请求处理任务已启动");

            try
            {
                await foreach (var i in _urlChannel.Reader.ReadAllAsync(cancellationToken))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await HandleInputAsync(i, cancellationToken);

                    logger.LogInformation("剩余任务: {count}", _urlChannel.Reader.Count);
                }
            }
            catch (OperationCanceledException)
            {
                List<Uri> remainingUrls = [];
                while (_urlChannel.Reader.TryRead(out var url)) remainingUrls.Add(url);

                using var _ = logger.BeginScope("RemainingUrls", $"[ {string.Join(',', remainingUrls)} ]");
                logger.LogWarning("请求处理任务已取消");
            }

        }, cancellationToken);
    }
    private async Task InputAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("请求输入任务已启动");

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                System.Console.CursorVisible = true;

                try
                {
                    var input = AsyncConsole.ReadLine(cancellationToken)?.Trim();
                    if (string.IsNullOrEmpty(input)) continue;


                    if (!Uri.TryCreate(input, UriKind.Absolute, out var url))
                    {
                        logger.LogWarning("请输入网址");
                        continue;
                    }
                    await _urlChannel.Writer.WriteAsync(url, cancellationToken);

                    using var _ = logger.BeginScope("Url", url);
                    logger.LogInformation("已加入消费队列");
                }
                catch (OperationCanceledException)
                {
                    logger.LogError("消费队列已关闭, 无法添加");
                }
            }
        }
        finally
        {
            _urlChannel.Writer.TryComplete();
            logger.LogInformation("请求通道已关闭");
        }
    }

    private async Task HandleInputAsync(Uri url, CancellationToken cancellationToken)
    {
        var requestId = Interlocked.Increment(ref _requestId);
        using var _ = logger.Scope("RequestId", requestId)
            .Add("Url", url)
            .Begin();

        var beginTime = Stopwatch.GetTimestamp();

        try
        {
            logger.LogInformation("开始处理请求");
            var source = urlSourceParserSelector
                       .Select(url)
                       .Parse(url);

            await mediaRelay.HandleAsync(source, cancellationToken);
            logger.LogInformation("请求处理完成, 耗时: {s}s", Stopwatch.GetElapsedTime(beginTime).TotalSeconds);
        }
        catch (NotSupportedException ex)
        {
            logger.LogWarning(ex, "不支持的请求");
        }
        catch (TimeoutException ex)
        {
            logger.LogWarning(ex, "请求超时");
        }
        catch (OperationCanceledException ex)
        {
            logger.LogWarning(ex, "请求取消");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "请求处理失败");
        }
    }
}