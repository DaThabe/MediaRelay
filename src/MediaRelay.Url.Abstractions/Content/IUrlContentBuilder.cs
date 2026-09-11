using MediaRelay.Resources;

namespace MediaRelay.Content;


public interface IUrlContentBuilder<TBuilder, TContent>
    where TBuilder : IUrlContentBuilder<TBuilder, TContent>
    where TContent : IUrlContent
{
    TBuilder SetTitle(string title);
    TBuilder SetContent(string content);
    TBuilder SetUploadTime(DateTimeOffset uploadTime);
    TBuilder SetAuthorName(string name);
    TBuilder SetAuthorLink(Uri url);
    TBuilder AddTags(params IEnumerable<string> tags);
    TBuilder AddResources(params IEnumerable<IResource> resources);

    TContent Build();
}
public static class UrlContentBuilderExtensions
{
    extension<TBuilder, TContent>(IUrlContentBuilder<TBuilder, TContent> builder)
        where TBuilder : IUrlContentBuilder<TBuilder, TContent>
        where TContent : IUrlContent
    {
        public TBuilder SetAuthorLink(string url)
        {
            return builder.SetAuthorLink(new Uri(url));
        }

        public TBuilder SetAuthor(string name, Uri url)
        {
            return builder.SetAuthorName(name).SetAuthorLink(url);
        }
        public TBuilder SetAuthor(string name, string url)
        {
            return builder.SetAuthorName(name).SetAuthorLink(url);
        }
    }
}