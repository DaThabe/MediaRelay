using MediaRelay.Content;
using MediaRelay.Source;
using MediaRelay.Storage;

namespace MediaRelay;


public sealed record class RelayPayload
{
    public required SourceId SourceId { get; init; }
    public required ContentId ContentId { get; init; }
    public required IReadOnlySet<StorageInfo> Resources { get; init; }


    public string? Title { get; init; }
    public string? Description { get; init; }
    public DateTimeOffset? UploadAt { get; init; }


    public string? Author { get; init; }
    public string? AuthorUrl { get; init; }
    public Uri? SourceUrl { get; init; }


    public IReadOnlySet<string> Tags { get; init; } = new HashSet<string>();
}