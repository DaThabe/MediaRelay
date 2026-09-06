using MediaRelay.Content;
using MediaRelay.Storage;

namespace MediaRelay.Publish;


public sealed record class PublishContent
{
    public required ContentId ContentId { get; init; }
    public required Uri SourceUrl { get; init; }
    public required IReadOnlySet<StorageInfo> Resources { get; init; }

    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTimeOffset UploadAt { get; init; }


    public string? Author { get; init; }
    public string? AuthorUrl { get; init; }


    public IReadOnlySet<string> Tags { get; init; } = new HashSet<string>();
}