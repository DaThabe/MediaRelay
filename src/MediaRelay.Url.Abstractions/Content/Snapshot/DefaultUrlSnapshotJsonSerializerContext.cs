using System.Text.Json.Serialization;

namespace MediaRelay.Content.Snapshot;


[JsonSourceGenerationOptions(
    // 忽略大小写，允许驼峰和帕斯卡命名
    PropertyNameCaseInsensitive = true,
    // 格式化输出
    WriteIndented = true
)]
[JsonSerializable(typeof(DefaultUrlSnapshot))]
internal partial class DefaultUrlSnapshotJsonSerializerContext : JsonSerializerContext;