namespace MediaRelay.Resources;


public interface IUrlResource : IResource
{
    Uri Url { get; }
}