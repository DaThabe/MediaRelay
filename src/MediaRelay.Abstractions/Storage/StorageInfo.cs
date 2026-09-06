namespace MediaRelay.Storage;

public record StorageInfo
{
    public required Uri Uri { get; init; }
    public required string Hash { get; init; }
    public required string HashAlgorithm { get; init; }
    public required long Size { get; init; }
    public required string Extensions { get; init; }
}
