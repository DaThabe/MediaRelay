using MediaRelay.Resources;
using MediaRelay.Source;
using MediaRelay.Source.Url;

namespace MediaRelay.Content;


/// <summary>
/// 表示一个内容, 内容可以包含多个媒体资源, 例如图片, 视频等
/// </summary>
public interface IContent
{
    ContentId Id { get; }
    ISource Source { get; }
    IReadOnlySet<IResource> MediaResources { get; }
}

public interface IWebContent : IContent
{
    ISource IContent.Source => Source;
    new IUrlSource Source { get; }
}