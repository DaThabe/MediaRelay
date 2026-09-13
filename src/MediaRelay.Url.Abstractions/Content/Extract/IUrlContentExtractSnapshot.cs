using System.Text.Json.Serialization;

namespace MediaRelay.Content.Extract;


/// <summary>
/// 网址内容提取快照
/// </summary>
public interface IUrlContentExtractSnapshot
{
    IReadOnlySet<string> Resources { get; }
    string? Title { get; }
    string? Content { get; }
    DateTimeOffset? UploadAt { get; }
    string? AuthorName { get; }
    string? AuthorUrl { get; }
    IReadOnlySet<string> Tags { get; }
}

/// <summary>
/// 网址内容提取快照
/// </summary>
public record class UrlContentExtractorSnapshot : IUrlContentExtractSnapshot
{
    public required string[] Resources { get; init; }
    public string? Title { get; init; }
    public string? Content { get; init; }
    public DateTimeOffset? UploadAt { get; init; }
    public string? AuthorName { get; init; }
    public string? AuthorUrl { get; init; }
    public string[] Tags{ get; init; } = [];


    [JsonIgnore] IReadOnlySet<string> IUrlContentExtractSnapshot.Resources => Resources.ToHashSet();
    [JsonIgnore] IReadOnlySet<string> IUrlContentExtractSnapshot.Tags => Tags.ToHashSet();
}