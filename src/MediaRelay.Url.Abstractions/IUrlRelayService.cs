using MediaRelay.Content;
using MediaRelay.Storage;

namespace MediaRelay;

/// <summary>
/// 从 Url 转发
/// </summary>
public interface IUrlRelayService
{
    ValueTask RelayAsync(Uri url, CancellationToken cancellationToken = default);
}


/// <summary>
/// 转发网址内容创建者
/// </summary>
/// <typeparam name="TUrlContent">网址内容类型</typeparam>
public class RelayUrlContentCreator<TUrlContent>(
    IResourceStorage resourceStorage) : IRelayContentCreator
    where TUrlContent : IUrlContent
{
    public bool CanCreate(IContent content) => content is TUrlContent;
    public async ValueTask<RelayPayload> CreateAsync(IContent content, CancellationToken cancellationToken = default)
    {
        if (content is not TUrlContent urlContent)
            throw new NotSupportedException($"不支持的网址内容: {content}");

        // 储存所有资源
        var resourceUris = await resourceStorage
            .StoreAllAsync(urlContent.Resources, cancellationToken);

        return new()
        {
            SourceId = urlContent.Source.Id,
            ContentId = urlContent.Id,

            Author = urlContent.AuthorName,
            AuthorUrl = urlContent.AuthorUrl,
            SourceUrl = urlContent.Source.Url,

            Title = urlContent.Content,
            UploadAt = urlContent.UploadAt,

            Tags = urlContent.Tags.ToHashSet(),
            Resources = resourceUris.Values.ToHashSet(),
        };
    }
}


/// <summary>
/// 转发网址内容创建者
/// </summary>
public class RelayUrlContentCreator(
    IResourceStorage resourceStorage) : RelayUrlContentCreator<UrlContent>(resourceStorage);