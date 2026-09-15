using MediaRelay.Content;
using MediaRelay.Metadata;
using MediaRelay.Source;
using MediaRelay.Storage.Media;

namespace MediaRelay.Payload;


public interface IPayload
{
    ContentId ContentId { get; }
    IReadOnlySet<MediaStorageInfo> Resources { get; }
    ISource Source { get; }
    IMetadata Metadata { get; }
}
