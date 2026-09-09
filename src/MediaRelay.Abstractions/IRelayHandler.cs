namespace MediaRelay;


/// <summary>
/// 转发内容处理者
/// </summary>
public interface IRelayHandler
{
    bool CanRelay(RelayContent content);
    ValueTask RelayAsync(RelayContent content, CancellationToken cancellationToken = default);
}