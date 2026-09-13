using MediaRelay.Content;
using MediaRelay.Storage;

namespace MediaRelay.Payload;


/// <inheritdoc/>
/// <typeparam name="TUrlContent">网址内容类型</typeparam>
public class UrlPayloadCreator<TUrlContent>(
    IResourceStorage resourceStorage) : IPayloadCreator
    where TUrlContent : IUrlContent
{
    /// <summary>
    /// 判断内容是否为 <typeparamref name="TUrlContent"/> 类型
    /// </summary>
    public bool CanCreate(IContent content) => content is TUrlContent;

    /// <summary>
    /// 创建转发数据，并存储内容中的所有资源
    /// </summary>
    /// <exception cref="NotSupportedException">不支持的网址内容类型</exception>
    public async ValueTask<IPayload> CreateAsync(IContent content, CancellationToken cancellationToken = default)
    {
        if (content is not TUrlContent urlContent)
            throw new NotSupportedException($"不支持的网址内容: {content}");

        // 储存所有资源
        var resourceUris = await resourceStorage
            .StoreAllAsync(urlContent.Resources, cancellationToken);

        return new UrlPayload()
        {
            Source = urlContent.Source,
            ContentId = urlContent.Id,
            Resources = resourceUris.Values.ToHashSet(),
            Metadata = urlContent.Metadata
        };
    }
}
