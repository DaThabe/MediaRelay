using MediaRelay.Source;
using Microsoft.Extensions.Logging;

namespace MediaRelay.Content;


internal sealed class UrlContentRepository : IUrlContentRepository
{
    private readonly Dictionary<SourceId, IUrlContent> _cache = [];


    public ValueTask<IUrlContent?> FindAsync(IUrlSource source, CancellationToken cancellationToken = default)
    {
        _cache.TryGetValue(source.Id, out var content);
        return new ValueTask<IUrlContent?>(content);
    }

    public ValueTask AddAsync(IUrlContent content, CancellationToken cancellationToken = default)
    {
        _cache.Add(content.Source.Id, content);

        return ValueTask.CompletedTask;
    }
}