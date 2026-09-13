using MediaRelay.Browser;
using MediaRelay.Content.Extract;
using MediaRelay.HeyBox.Image;
using MediaRelay.Http;
using MediaRelay.Resource;
using Microsoft.Extensions.Options;

namespace MediaRelay.HeyBox.BbsLink;


internal sealed class LinkContentExtractor(
        IBrowserService browserService,
        OriginalImageUrl.Parser parser,
        OriginalImageUrlResource.Factory factory,
        IOptions<HeyBoxOptions> options
    ) : UrlContentExtractor<LinkSource>(browserService)
{
    protected override IEnumerable<HttpCookieOptions> GetCookies() =>
        options.Value.Http.Cookies;

    protected override string GetScriptFilePath() =>
        options.Value.BbsLink.ExtractScriptPath;

    protected override IResource ToResource(string resourceUrl) =>
        factory.Create(parser.Parse(resourceUrl));

}