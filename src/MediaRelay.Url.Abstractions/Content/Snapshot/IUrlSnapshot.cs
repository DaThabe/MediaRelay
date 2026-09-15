namespace MediaRelay.Content.Snapshot;


/// <summary>
/// 网址内容提取快照
/// </summary>
public interface IUrlSnapshot
{
    IReadOnlySet<Uri> Resources { get; }

    string? Title { get; }
    string? Content { get; }

    string? AuthorName { get; }
    Uri? AuthorUrl { get; }
    DateTimeOffset? UploadAt { get; }

    IReadOnlySet<string> Tags { get; }
}