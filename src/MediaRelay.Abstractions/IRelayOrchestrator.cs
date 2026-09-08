namespace MediaRelay;


/// <summary>
/// 推送调度器
/// </summary>
public interface IRelayOrchestrator
{
    ValueTask RelayAsync(RelayContent content, CancellationToken cancellationToken = default);
}