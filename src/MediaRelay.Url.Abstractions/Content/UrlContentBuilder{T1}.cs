namespace MediaRelay.Content;


/// <summary>
/// 自定义网址内容构建器
/// </summary>
/// <typeparam name="TUrlContent">网址内容类型</typeparam>
public abstract class UrlContentBuilder<TUrlContent> : UrlContentBuilder<UrlContentBuilder<TUrlContent>, TUrlContent>
    where TUrlContent : IUrlContent;
