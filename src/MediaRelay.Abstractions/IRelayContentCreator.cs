using MediaRelay.Content;

namespace MediaRelay;


/// <summary>
/// 转发内容创建者
/// </summary>
public interface IRelayContentCreator
{
    bool CanCreate(IContent content);
    ValueTask<RelayContent> CreateAsync(IContent content, CancellationToken cancellationToken = default);
}