using MediaRelay.Content;
using MediaRelay.Storage;

namespace MediaRelay;

/// <summary>
/// 从 Url 转发
/// </summary>
public interface IUrlRelayService
{
    ValueTask RelayAsync(Uri url, CancellationToken cancellationToken = default);
}


/// <summary>
/// 转发网址内容创建者
/// </summary>
public class RelayUrlContentCreator(
    IResourceStorage resourceStorage) : RelayUrlContentCreator<UrlContent>(resourceStorage);