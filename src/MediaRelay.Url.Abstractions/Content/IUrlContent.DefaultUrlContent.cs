using MediaRelay.Metadata;
using MediaRelay.Resource;
using MediaRelay.Source;

namespace MediaRelay.Content;


public record class DefaultUrlContent : IUrlContent
{
    public required ContentId Id { get; init; }
    public required IUrlSource Source { get; init; }
    public IUrlMetadata Metadata { get; init; } = DefaultUrlMetadata.Empty;
    public required IReadOnlySet<IUrlResource> Resources { get; init; }
}