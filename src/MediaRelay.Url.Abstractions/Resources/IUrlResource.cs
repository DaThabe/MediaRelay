using MediaRelay.Resources;

namespace MediaRelay.Url.Resources;


public interface IUrlResource : IResource
{
    Uri Url { get; }
}