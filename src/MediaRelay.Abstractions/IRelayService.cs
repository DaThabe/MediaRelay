using MediaRelay.Payload;

namespace MediaRelay;


/// <summary>
/// 将转发数据转发
/// </summary>
public interface IRelayService
{
    /// <summary>
    /// 转发数据
    /// </summary>
    ValueTask RelayAsync(RelayPayload payload, CancellationToken cancellationToken = default);
}