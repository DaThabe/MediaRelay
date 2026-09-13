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
