using MediaRelay.Content;
using MediaRelay.Metadata;
using MediaRelay.Source;
using MediaRelay.Storage;

namespace MediaRelay.Payload;


public interface IUrlPayload : IPayload
{
    IMetadata IPayload.Metadata => Metadata;
    ISource IPayload.Source => Source;


    new IUrlSource Source { get; }
    new IUrlMetadata Metadata { get; }
}


public record class DefaultUrlPayload : IUrlPayload
{
    public required ContentId ContentId { get; init; }
    public required IReadOnlySet<StorageInfo> Resources { get; init; }
    public required IUrlSource Source { get; init; }
    public IUrlMetadata Metadata { get; init; } = DefaultUrlMetadata.Empty;
}
