namespace MediaRelay;


/// <summary>
/// 转发
/// </summary>
public interface IRelayService
{
    ValueTask RelayAsync(RelayContent content, CancellationToken cancellationToken = default);
}