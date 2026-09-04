using MediaRelay.Content;

namespace MediaRelay.Publish;


/// <summary>
/// 根据内容选合适的处理器
/// </summary>
public interface IPublishContentConverterSelector
{
    IPublishContentConverter Select(IContent content);
}