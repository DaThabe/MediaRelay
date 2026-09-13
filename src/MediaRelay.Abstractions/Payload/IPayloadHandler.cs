namespace MediaRelay.Payload;


/// <summary>
/// 转发数据处理器
/// </summary>
public interface IPayloadHandler
{
    /// <summary>
    /// 该数据是能转发
    /// </summary>
    bool CanHandle(IPayload payload);

    /// <summary>
    /// 转发数据
    /// </summary>
    ValueTask HandleAsync(IPayload payload, CancellationToken cancellationToken = default);
}