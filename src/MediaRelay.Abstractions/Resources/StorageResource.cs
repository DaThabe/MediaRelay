namespace MediaRelay.Resources;


public sealed record class StorageResource
{
    public required Uri Uri { get; init; }
    public required string Hash { get; init; }
    public required string HashAlgorithm { get; init; }
    public required long Size { get; init; }
    public required string Extensions { get; init; }
}