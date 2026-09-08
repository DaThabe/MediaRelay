using MediaRelay.Source;

namespace MediaRelay;


/// <summary>
/// 来源转发
/// </summary>
public interface ISourceRelayService
{
    ValueTask RelayAsync(ISource source, CancellationToken cancellationToken = default);
}