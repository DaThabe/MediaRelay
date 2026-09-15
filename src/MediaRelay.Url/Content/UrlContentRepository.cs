using MediaRelay.Source;
using Microsoft.Extensions.Logging;

namespace MediaRelay.Content;


internal sealed class UrlContentRepository(ILogger<UrlContentRepository> logger) : IUrlContentRepository
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

        using var _ = logger.BeginScope("ContentId", content.Id);
        logger.LogTrace("添加了网址内容");

        return ValueTask.CompletedTask;
    }
}