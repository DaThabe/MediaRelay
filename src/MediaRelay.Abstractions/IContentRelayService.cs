using MediaRelay.Content;

namespace MediaRelay;

/// <summary>
/// 内容转发
/// </summary>
public interface IContentRelayService
{
    ValueTask RelayAsync(IContent content, CancellationToken cancellationToken = default);
}