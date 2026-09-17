using MediaRelay.Content;

namespace MediaRelay;


public interface IContentRelayService
{
    ValueTask RelayAsync(IContent content, CancellationToken cancellationToken = default);
}