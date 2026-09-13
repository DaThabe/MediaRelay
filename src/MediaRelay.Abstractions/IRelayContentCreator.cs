using MediaRelay.Content;

namespace MediaRelay;


/// <summary>
/// 从内容创建转发内容
/// </summary>
public interface IRelayContentCreator
{
    bool CanCreate(IContent content);
    ValueTask<RelayPayload> CreateAsync(IContent content, CancellationToken cancellationToken = default);
}