using MediaRelay.Content;
using MediaRelay.Metadata;
using MediaRelay.Source;
using MediaRelay.Storage;

namespace MediaRelay.Payload;


public interface IPayload
{
    ContentId ContentId { get; }
    IReadOnlySet<StorageInfo> Resources { get; }
    ISource Source { get; }
    IMetadata Metadata { get; }
}


public record class DefaultPayload : IPayload
{
    public required ContentId ContentId { get; init; }
    public required ISource Source { get; init; }
    public required IReadOnlySet<StorageInfo> Resources { get; init; }
    public required IMetadata Metadata { get; init; }
}