using MediaRelay.Content;

namespace MediaRelay.Payload;


internal sealed class PayloadFactory(
        IEnumerable<IPayloadCreator> relayContentCreators
    ) : IPayloadFactory
{
    private readonly IPayloadCreator[] _createtors = [.. relayContentCreators];

    public async ValueTask<IPayload> CreateAsync(IContent content, CancellationToken cancellationToken = default)
    {
        foreach (var creator in _createtors)
        {
            if (!creator.CanCreate(content)) continue;
            return await creator.CreateAsync(content, cancellationToken);
        }

        throw new NotSupportedException($"无法提取该输入: {content.GetType().Name}");
    }
}