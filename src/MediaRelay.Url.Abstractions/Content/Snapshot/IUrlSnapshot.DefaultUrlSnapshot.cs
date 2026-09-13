using MediaRelay.Metadata;
using System.Text.Json.Serialization;

namespace MediaRelay.Content.Snapshot;


public record class DefaultUrlSnapshot : IUrlSnapshot
{
    public required string[] Resources { get; init; }
    public string? Title { get; init; }
    public string? Content { get; init; }
    public DateTimeOffset? UploadAt { get; init; }
    public string? AuthorName { get; init; }
    public string? AuthorUrl { get; init; }
    public string[] Tags { get; init; } = [];



    [JsonIgnore]
    IReadOnlySet<string> IUrlSnapshot.Resources => Resources.ToHashSet();

    [JsonIgnore]
    IUrlMetadata IUrlSnapshot.Metadata => new DefaultUrlMetadata()
    {
        AuthorName = AuthorName,
        AuthorUrl = AuthorUrl is null ? null : new Uri(AuthorUrl),
        Title = Title,
        Description = Content,
        PublishedAt = UploadAt,
        Tags = Tags.ToHashSet()
    };
}
