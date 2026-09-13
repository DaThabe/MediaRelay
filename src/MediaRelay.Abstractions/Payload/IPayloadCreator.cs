using MediaRelay.Content;

namespace MediaRelay.Payload;


/// <summary>
/// 从内容创建转发内容
/// </summary>
public interface IPayloadCreator
{
    /// <summary>
    /// 判断内容是否能创建转发数据
    /// </summary>
    bool CanCreate(IContent content);

    /// <summary>
    /// 使用内容创建转发数据
    /// </summary>
    ValueTask<IPayload> CreateAsync(IContent content, CancellationToken cancellationToken = default);
}