namespace MediaRelay.Payload;


/// <summary>
/// 转发数据处理器
/// </summary>
public interface IPayloadHandler
{
    /// <summary>
    /// 该数据是能转发
    /// </summary>
    bool CanRelay(IPayload payload);

    /// <summary>
    /// 转发数据
    /// </summary>
    ValueTask RelayAsync(IPayload payload, CancellationToken cancellationToken = default);
}