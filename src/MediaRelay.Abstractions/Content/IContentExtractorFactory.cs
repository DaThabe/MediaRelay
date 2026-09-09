using MediaRelay.Source;

namespace MediaRelay.Content;

public interface IContentExtractorFactory
{
    ValueTask<IContent> CreateAsync(ISource source, CancellationToken cancellationToken = default);
}