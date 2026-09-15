using MediaRelay.Storage.Media;

namespace MediaRelay.Resource;


public interface IUrlResource : IResource
{
    Uri Url { get; }
}

public abstract class UrlResource : IUrlResource
{
    public required ResourceId Id { get; init; }
    public required Uri Url { get; init; }
    public required MediaType Type { get; init; }


    public abstract ValueTask<Stream> GetStreamAsync(CancellationToken cancellationToken = default);
}