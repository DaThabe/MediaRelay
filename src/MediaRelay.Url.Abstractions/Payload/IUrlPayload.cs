using MediaRelay.Metadata;
using MediaRelay.Source;

namespace MediaRelay.Payload;


public interface IUrlPayload : IPayload
{
    IMetadata IPayload.Metadata => Metadata;
    ISource IPayload.Source => Source;


    new IUrlSource Source { get; }
    new IUrlMetadata Metadata { get; }
}
