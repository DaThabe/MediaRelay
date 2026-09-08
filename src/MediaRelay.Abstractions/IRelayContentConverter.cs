using MediaRelay.Content;

namespace MediaRelay;


/// <summary>
/// 转发内容转换器
/// </summary>
public interface IRelayContentConverter
{
    bool CanConvert(IContent content);
    ValueTask<RelayContent> ConvertAsync(IContent content, CancellationToken cancellationToken = default);
}