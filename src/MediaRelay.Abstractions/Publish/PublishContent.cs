using MediaRelay.Content;
using MediaRelay.Resources;

namespace MediaRelay.Publish;


public sealed record class PublishContent
{
    public required ContentId ContentId { get; init; }
    public required Uri SourceUri { get; init; }
    public required IReadOnlySet<StorageResource> Resources { get; init; }

    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DateTimeOffset UploadAt { get; init; }


    public string? Author { get; init; }
    public string? AuthorUrl { get; init; }


    public IReadOnlySet<string> Tags { get; init; } = new HashSet<string>();
}