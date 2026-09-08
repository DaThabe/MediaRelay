using MediaRelay.Content;

namespace MediaRelay;


/// <summary>
/// 根据内容选合适的处理器
/// </summary>
public interface IRelayContentConverterSelector
{
    IRelayContentConverter Select(IContent content);
}