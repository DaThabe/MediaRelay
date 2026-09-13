namespace MediaRelay.Payload;


/// <summary>
/// 转发数据处理器
/// </summary>
public interface IRelayPayloadHandler
{
    /// <summary>
    /// 该数据是能转发
    /// </summary>
    bool CanRelay(RelayPayload payload);

    /// <summary>
    /// 转发数据
    /// </summary>
    ValueTask RelayAsync(RelayPayload payload, CancellationToken cancellationToken = default);
}