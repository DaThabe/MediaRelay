using AsyncConsoleReader;
using MediaRelay.Source.Url;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace MediaRelay.Console.Input;


internal sealed class InputListenBackgroundService(
        IMediaRelay mediaRelay,
        IInputUrlBuffer inputUrlBuffer,
        IUrlSourceParserSelector urlSourceParserSelector,
        ILogger<InputListenBackgroundService> logger
    ) : BackgroundService
{
    private int _requestId;


    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _ = ConsumeAsync(stoppingToken);
        return InputAsync(stoppingToken);
    }

    private Task ConsumeAsync(CancellationToken cancellationToken)
    {
        return Task.Run(async () =>
        {
            logger.LogInformation("请求处理任务已启动");

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var url = await inputUrlBuffer.WaitPeepAsync(cancellationToken);
                    await HandleInputAsync(url, cancellationToken);

                    var removeUrl = await inputUrlBuffer.WaitReadAsync(cancellationToken);
                    logger.LogInformation("剩余任务: {count}", inputUrlBuffer.Count);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "输入消费队列任务异常");
            }

        }, cancellationToken);
    }
    private async Task InputAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("请求输入任务已启动");

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
                await inputUrlBuffer.WriteAsync(url, cancellationToken);

                using var _ = logger.BeginScope("Url", url);
                logger.LogInformation("已加入消费队列");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "消费队列已关闭, 无法添加");
            }
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