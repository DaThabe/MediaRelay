using MediaRelay.Source;

namespace MediaRelay.Content;


/// <summary>
/// 根据来源选择合适的内容提取器
/// </summary>
public interface IContentExtractorSelector
{
    IContentExtractor Select(ISource source);
}