namespace MediaRelay.Content;


/// <summary>
/// 转发内容转换器
/// </summary>
public interface IContentConverter
{
    bool CanConvert(IContent content);
    ValueTask<RelayContent> ConvertAsync(IContent content, CancellationToken cancellationToken = default);
}