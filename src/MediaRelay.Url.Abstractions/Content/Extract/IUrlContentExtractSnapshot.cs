using MediaRelay.Metadata;
using System.Text.Json.Serialization;

namespace MediaRelay.Content.Extract;


/// <summary>
/// 网址内容提取快照
/// </summary>
public interface IUrlContentExtractSnapshot
{
    IReadOnlySet<string> Resources { get; }
    IUrlMetadata Metadata { get; }
}

/// <summary>
/// 网址内容提取快照
/// </summary>
public record class DefaultUrlContentExtractorSnapshot : IUrlContentExtractSnapshot
{
    public required string[] Resources { get; init; }
    public string? Title { get; init; }
    public string? Content { get; init; }
    public DateTimeOffset? UploadAt { get; init; }
    public string? AuthorName { get; init; }
    public string? AuthorUrl { get; init; }
    public string[] Tags { get; init; } = [];



    [JsonIgnore]
    IReadOnlySet<string> IUrlContentExtractSnapshot.Resources => Resources.ToHashSet();


    [JsonIgnore]
    IUrlMetadata IUrlContentExtractSnapshot.Metadata => new DefaultUrlMetadata()
    {
        AuthorName = AuthorName,
        AuthorUrl = AuthorUrl is null ? null : new Uri(AuthorUrl),
        Title = Title,
        Description = Content,
        PublishedAt = UploadAt,
        Tags = Tags.ToHashSet()
    };
}