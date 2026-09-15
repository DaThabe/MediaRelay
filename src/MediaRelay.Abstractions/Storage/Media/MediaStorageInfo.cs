using MediaRelay.Storage.Hash;

namespace MediaRelay.Storage.Media;


public record MediaStorageInfo
{
    public required Uri Uri { get; init; }
    public required MediaType MediaType { get; init; }
    public required long Size { get; init; }
    public required HashInfo HashInfo { get; init; }
}