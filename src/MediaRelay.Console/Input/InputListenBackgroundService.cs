using AsyncConsoleReader;
using MediaRelay.Persistent;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace MediaRelay.Console.Input;


internal sealed class InputListenBackgroundService(
        IUrlRelayService urlRelay,
        IUrlPersistentQueueFactory urlPersistentQueueFactory,
        ILogger<InputListenBackgroundService> logger
    ) : BackgroundService
{
    private int _requestId;


    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _ = ConsumeAsync(stoppingToken);
        _ = InputAsync(stoppingToken);

        return Task.CompletedTask;
    }

    private async Task ConsumeAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("请求处理任务已启动");
        var urlPersistentQueue = await urlPersistentQueueFactory.GetOrCreateAsync(cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            var url = await urlPersistentQueue.PeepWaitAsync(cancellationToken);
            var requestId = Interlocked.Increment(ref _requestId);
            var beginTime = Stopwatch.GetTimestamp();

            using var _ = logger.Scope("RequestId", requestId)
                .Add("Url", url)
                .Begin();

            try
            {
                await urlRelay.RelayAsync(url, cancellationToken);
                await urlPersistentQueue.ReadWaitAsync(cancellationToken);

                using var __ = logger.BeginScope("ElapsedTime", Stopwatch.GetElapsedTime(beginTime));
                logger.LogInformation("请求处理完成");
            }
            catch (Exception ex)
            {
                try
                {
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                    await urlPersistentQueue.WriteAsync(url, cts.Token);
                    logger.LogError(ex, "请求处理失败, 已重新追加至队列");
                }
                catch (Exception writeEx)
                {
                    logger.LogCritical(writeEx, "无法储存至队列");
                }
            }
        }
    }
    private async Task InputAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("网址输入任务已启动");
        var urlPersistentQueue = await urlPersistentQueueFactory.GetOrCreateAsync(cancellationToken);

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

                await urlPersistentQueue.WriteAsync(url, cancellationToken);

                using var _ = logger.BeginScope("Url", url);
                logger.LogInformation("已加入持久队列");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "无法处理输入");
            }
        }
    }
}