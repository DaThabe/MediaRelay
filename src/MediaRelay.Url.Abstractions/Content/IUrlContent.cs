using MediaRelay.Metadata;
using MediaRelay.Source;

namespace MediaRelay.Content;


public interface IUrlContent : IContent
{
    ISource IContent.Source => Source;
    IMetadata IContent.Metadata => Metadata;


    new IUrlSource Source { get; }
    new IUrlMetadata Metadata { get; }
}
