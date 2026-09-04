using MediaRelay.Content;

namespace MediaRelay.Publish;


/// <summary>
/// 发布内容转换器
/// </summary>
public interface IPublishContentConverter
{
    bool CanConvert(IContent content);
    ValueTask<PublishContent> ConvertAsync(IContent content, CancellationToken cancellationToken = default);
}