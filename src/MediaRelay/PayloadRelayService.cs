using MediaRelay.Payload;
using Microsoft.Extensions.Logging;

namespace MediaRelay;


internal sealed class PayloadRelayService(
    IEnumerable<IPayloadHandler> payloadHandlers,
    ILogger<PayloadRelayService> logger
    ) : IPayloadRelayService
{
    private readonly IPayloadHandler[] _payloadHandlers = [.. payloadHandlers];

    public async ValueTask RelayAsync(IPayload payload, CancellationToken cancellationToken = default)
    {
        var canExecuteHandlers = _payloadHandlers.Where(x => x.CanHandle(payload)).ToArray();
        if (canExecuteHandlers.Length == 0) return;


        if (logger.IsEnabled(LogLevel.Debug))
            logger.LogDebug("等待转发, 共 {Count} 个可执行处理器", canExecuteHandlers.Length);

        int count = 0;
        foreach (var handler in canExecuteHandlers)
        {
            try
            {
                await handler.HandleAsync(payload, cancellationToken);
                count++;

                if (logger.IsEnabled(LogLevel.Debug))
                    logger.LogDebug("[{HandlerName}] 转发完成", handler.GetType().Name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[{HandlerName}] 转发失败", handler.GetType().Name);
            }
        }

        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("转发结束, 已执行处理器 [{Count}/{Total}]", count, canExecuteHandlers.Length);
    }
}
