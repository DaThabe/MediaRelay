using MediaRelay.Content;

namespace MediaRelay.Payload;


/// <summary>
/// 从内容创建转发数据
/// </summary>
public interface IPayloadFactory
{
    /// <summary>
    /// 从内容创建转发数据
    /// </summary>
    ValueTask<IPayload> CreateAsync(IContent content, CancellationToken cancellationToken = default);
}