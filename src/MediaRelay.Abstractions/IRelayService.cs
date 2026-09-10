namespace MediaRelay;


/// <summary>
/// 转发内容
/// </summary>
public interface IRelayService
{
    ValueTask RelayAsync(RelayPayload payload, CancellationToken cancellationToken = default);
}