namespace MediaRelay.Content;


/// <summary>
/// 根据内容选合适的处理器
/// </summary>
public interface IContentConverterSelector
{
    IContentConverter Select(IContent content);
}