using MediaRelay.Browser;
using MediaRelay.Content.Extract;
using MediaRelay.Http;
using MediaRelay.Pixiv.Image;
using MediaRelay.Serializer;
using Microsoft.Extensions.Options;

namespace MediaRelay.Pixiv.Artwork;


internal sealed class ArtworkContentExtractor(
        IPageSessionFactory pageSessionFactory,
        IJsonSerializerFactory jsonSerializerFactory,
        OriginalImageUrl.Parser parser,
        OriginalImageUrlResource.Factory factory,
        IOptions<PixivOptions> options
    ) : UrlSourceContentExtractor<ArtworkSource>
{
    protected override IPageSessionFactory PageSessionFactory { get; } = pageSessionFactory;
    protected override IJsonSerializerFactory JsonSerializerFactory { get; } = jsonSerializerFactory;
    protected override IReadOnlySet<HttpCookieOptions> Cookies { get; } = options.Value.Http.Cookies.ToHashSet().AsReadOnly();
    protected override string ScriptFilePath { get; } = options.Value.Artwork.ExtractScriptPath;
    protected override UrlResourceParserHandler UrlResourceParser { get; } = url => factory.Create(parser.Parse(url));
}