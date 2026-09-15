using MediaRelay.Source;

namespace MediaRelay.Content;

public interface IUrlContentRepository
{
    ValueTask<IUrlContent?> FindAsync(IUrlSource source, CancellationToken cancellationToken = default);
    ValueTask AddAsync(IUrlContent content, CancellationToken cancellationToken = default);
}