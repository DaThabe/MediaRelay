namespace MediaRelay.Storage;


public record StorageInfo
{
    public required Uri Uri { get; init; }
    public required MediaType MediaType { get; init; }
    public required long Size { get; init; }
    public required HashInfo HashInfo { get; init; }
}