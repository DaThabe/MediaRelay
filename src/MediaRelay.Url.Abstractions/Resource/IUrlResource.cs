namespace MediaRelay.Resource;


public interface IUrlResource : IResource
{
    Uri Url { get; }
}