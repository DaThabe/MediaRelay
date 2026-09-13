using MediaRelay.Metadata;

namespace MediaRelay.Content.Snapshot;


/// <summary>
/// 网址内容提取快照
/// </summary>
public interface IUrlSnapshot
{
    IReadOnlySet<string> Resources { get; }
    IUrlMetadata Metadata { get; }
}