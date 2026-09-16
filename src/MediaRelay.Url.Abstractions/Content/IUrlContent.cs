using MediaRelay.Metadata;
using MediaRelay.Resource;
using MediaRelay.Source;

namespace MediaRelay.Content;


public interface IUrlContent : IContent
{
    ISource IContent.Source => Source;
    IMetadata IContent.Metadata => Metadata;

    [Obsolete("没办法协变, 所以每次都创建新的HashSet")]
    IReadOnlySet<IResource> IContent.Resources => new HashSet<IResource>(Resources);


    new IUrlSource Source { get; }
    new IUrlMetadata Metadata { get; }
    new IReadOnlySet<IUrlResource> Resources { get; }
}