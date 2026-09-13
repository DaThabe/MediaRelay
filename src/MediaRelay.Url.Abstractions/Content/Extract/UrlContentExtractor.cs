using MediaRelay.Browser;
using MediaRelay.Source;

namespace MediaRelay.Content.Extract;

/// <summary>
/// 默认网址内容提取器
/// </summary>
public abstract class UrlContentExtractor(IBrowserService browserService) : 
    UrlContentExtractor<DefaultUrlSource>(browserService);