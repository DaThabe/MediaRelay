using MediaRelay.Content;

namespace MediaRelay;


/// <summary>
/// 转发内容创建工厂
/// </summary>
public interface IRelayContentFactory
{
    ValueTask<RelayPayload> CreateAsync(IContent content, CancellationToken cancellationToken = default);
}