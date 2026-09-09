using MediaRelay.Content;
using Microsoft.Extensions.Logging;

namespace MediaRelay;


internal sealed class RelayContentFactory(
        IEnumerable<IRelayContentCreator> relayContentCreators,
        ILogger<RelayContentFactory> logger
    ) : IRelayContentFactory
{
    private readonly IRelayContentCreator[] _createtors = [.. relayContentCreators];

    public async ValueTask<RelayContent> CreateAsync(IContent content, CancellationToken cancellationToken = default)
    {
        foreach (var creator in _createtors)
        {
            if (!creator.CanCreate(content)) continue;

            using var _ = logger.BeginScope("Creator", creator.GetType().Name);
            logger.LogDebug("正在创建");

            return await creator.CreateAsync(content, cancellationToken);
        }

        throw new NotSupportedException($"无法提取该输入: {content.GetType().Name}");
    }
}