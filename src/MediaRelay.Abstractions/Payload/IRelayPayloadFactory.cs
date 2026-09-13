using MediaRelay.Content;

namespace MediaRelay.Payload;


/// <summary>
/// 从内容创建转发数据
/// </summary>
public interface IRelayPayloadFactory
{
    /// <summary>
    /// 从内容创建转发数据
    /// </summary>
    ValueTask<RelayPayload> CreateAsync(IContent content, CancellationToken cancellationToken = default);
}