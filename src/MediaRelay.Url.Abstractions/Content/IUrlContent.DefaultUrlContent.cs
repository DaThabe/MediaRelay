using MediaRelay.Metadata;
using MediaRelay.Resource;
using MediaRelay.Source;

namespace MediaRelay.Content;


public record class DefaultUrlContent : IUrlContent
{
    public required ContentId Id { get; init; }
    public required IUrlSource Source { get; init; }
    public required IReadOnlySet<IResource> Resources { get; init; }
    public IUrlMetadata Metadata { get; init; } = DefaultUrlMetadata.Empty;
}