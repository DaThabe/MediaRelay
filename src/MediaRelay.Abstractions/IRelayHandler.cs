namespace MediaRelay;


/// <summary>
/// 转发内容处理者
/// </summary>
public interface IRelayHandler
{
    bool CanRelay(RelayPayload payload);
    ValueTask RelayAsync(RelayPayload payload, CancellationToken cancellationToken = default);
}