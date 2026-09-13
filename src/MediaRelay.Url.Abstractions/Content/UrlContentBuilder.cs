using MediaRelay.Source;

namespace MediaRelay.Content;


/// <summary>
/// 默认网址内容构建器
/// </summary>
/// <param name="contentId">内容Id</param>
/// <param name="urlSource">网址来源信息</param>
public class UrlContentBuilder(ContentId contentId, IUrlSource urlSource) : UrlContentBuilder<UrlContentBuilder, UrlContent>
{
    protected override UrlContentBuilder This() => this;

    protected override UrlContent Build(Snapshot snapshot)
    {
        return new UrlContent()
        {
            Id = contentId,
            Source = urlSource,
            Resources = snapshot.Resources,
            Title = snapshot.Title,
            AuthorName = snapshot.AuthorName,
            AuthorUrl = snapshot.AuthorUrl,
            Content = snapshot.Content,
            Tags = snapshot.Tags,
            UploadAt = snapshot.UploadAt
        };
    }
}