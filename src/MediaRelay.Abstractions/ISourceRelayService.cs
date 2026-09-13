using MediaRelay.Source;

namespace MediaRelay;


public interface ISourceRelayService
{
    ValueTask RelayAsync(ISource source, CancellationToken cancellationToken = default);
}