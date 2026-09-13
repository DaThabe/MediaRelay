using MediaRelay.Metadata;
using MediaRelay.Resource;
using MediaRelay.Source;

namespace MediaRelay.Content;


public interface IUrlContent : IContent
{
    ISource IContent.Source => Source;
    IMetadata IContent.Metadata => Metadata;


    new IUrlSource Source { get; }
    new IUrlMetadata Metadata { get; }
}


public record class DefaultUrlContent : IUrlContent
{
    public required ContentId Id { get; init; }
    public required IUrlSource Source { get; init; }
    public required IReadOnlySet<IResource> Resources { get; init; }
    public IUrlMetadata Metadata { get; init; } = DefaultUrlMetadata.Empty;
}