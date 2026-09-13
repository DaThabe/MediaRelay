using MediaRelay.Browser;
using MediaRelay.Source;

namespace MediaRelay.Content.Extract;


/// <summary>
/// 自定网址内容义构建器的提取器
/// </summary>
/// <typeparam name="TUrlContentBuilder">网址内容构建器</typeparam>
/// <typeparam name="TUrlContent">网址内容</typeparam>
public abstract class UrlContentExtractor<TUrlContentBuilder, TUrlContent>(IBrowserService browserService) :
    UrlContentExtractor<UrlSource, UrlContentExtractorSnapshot, TUrlContentBuilder, TUrlContent>(browserService)
    where TUrlContentBuilder : IUrlContentBuilder<TUrlContentBuilder, TUrlContent>
    where TUrlContent : IUrlContent;