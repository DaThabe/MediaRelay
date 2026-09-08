namespace MediaRelay;


/// <summary>
/// 转发调度器
/// </summary>
public interface IRelayOrchestrator
{
    ValueTask RelayAsync(RelayContent content, CancellationToken cancellationToken = default);
}