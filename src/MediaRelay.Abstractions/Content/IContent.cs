using MediaRelay.Metadata;
using MediaRelay.Resource;
using MediaRelay.Source;

namespace MediaRelay.Content;


/// <summary>
/// 表示一个内容, 内容可以包含多个媒体资源, 例如图片, 视频等
/// </summary>
public interface IContent
{
    ContentId Id { get; }
    ISource Source { get; }
    IReadOnlySet<IResource> Resources { get; }
    IMetadata Metadata { get; }
}


public record class DefaultContent : IContent
{
    public required ContentId Id { get; init; }
    public required ISource Source { get; init; }
    public required IReadOnlySet<IResource> Resources { get; init; }
    public required IMetadata Metadata { get; init; }
}