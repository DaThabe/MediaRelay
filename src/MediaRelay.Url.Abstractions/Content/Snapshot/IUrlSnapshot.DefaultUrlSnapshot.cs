using System.Text.Json.Serialization;

namespace MediaRelay.Content.Snapshot;


public record class DefaultUrlSnapshot : IUrlSnapshot
{
    public required HashSet<Uri> Resources { get; init; }

    public string? Title { get; init => field = string.IsNullOrEmpty(value) ? null : value.Trim(); }
    public string? Content { get; init => field = string.IsNullOrEmpty(value) ? null : value.Trim(); }

    public Uri? AuthorUrl { get; init; }
    public string? AuthorName { get; init => field = string.IsNullOrEmpty(value) ? null : value.Trim(); }
    public DateTimeOffset? UploadAt { get; init; }

    public HashSet<string> Tags { get; init => field = [.. value.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim())]; } = [];



    [JsonIgnore] IReadOnlySet<Uri> IUrlSnapshot.Resources => Resources;
    [JsonIgnore] IReadOnlySet<string> IUrlSnapshot.Tags => Tags;
}


[JsonSourceGenerationOptions(
    // 忽略大小写，允许驼峰和帕斯卡命名
    PropertyNameCaseInsensitive = true,
    // 格式化输出
    WriteIndented = true
)]
[JsonSerializable(typeof(DefaultUrlSnapshot))]
internal partial class DefaultUrlSnapshotJsonSerializerContext : JsonSerializerContext;