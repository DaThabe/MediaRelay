using MediaRelay.Resources;

namespace MediaRelay.Url;


public interface IUrlResource : IResource
{
    Uri Url { get; }
}