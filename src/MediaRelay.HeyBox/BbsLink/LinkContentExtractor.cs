using MediaRelay.Browser;
using MediaRelay.Content.Extract;
using MediaRelay.HeyBox.Image;
using MediaRelay.Http;
using MediaRelay.Serializer;
using Microsoft.Extensions.Options;

namespace MediaRelay.HeyBox.BbsLink;


internal sealed class LinkContentExtractor(
        IPageSessionFactory pageSessionFactory,
        IJsonSerializerFactory jsonSerializerFactory,
        OriginalImageUrl.Parser parser,
        OriginalImageUrlResource.Factory factory,
        IOptions<HeyBoxOptions> options
    ) : UrlSourceContentExtractor<LinkSource>
{
    protected override IPageSessionFactory PageSessionFactory { get; } = pageSessionFactory;
    protected override IJsonSerializerFactory JsonSerializerFactory { get; } = jsonSerializerFactory;
    protected override IReadOnlySet<HttpCookieOptions> Cookies { get; } = options.Value.Http.Cookies.ToHashSet().AsReadOnly();
    protected override string ScriptFilePath { get; } = options.Value.BbsLink.ExtractScriptPath;
    protected override UrlResourceParserHandler UrlResourceParser { get; } = url => factory.Create(parser.Parse(url));

}