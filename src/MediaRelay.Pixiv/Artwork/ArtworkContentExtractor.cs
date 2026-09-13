using MediaRelay.Browser;
using MediaRelay.Content.Extract;
using MediaRelay.Http;
using MediaRelay.Pixiv.Image;
using MediaRelay.Resource;
using Microsoft.Extensions.Options;

namespace MediaRelay.Pixiv.Artwork;


internal sealed class ArtworkContentExtractor(
        IBrowserService browserService,
        OriginalImageUrl.Parser parser,
        OriginalImageUrlResource.Factory factory,
        IOptions<PixivOptions> options
    ) : UrlSourceContentExtractor<ArtworkSource>(browserService)
{
    protected override IEnumerable<HttpCookieOptions> GetCookies() =>
        options.Value.Http.Cookies;

    protected override string GetScriptFilePath() =>
        options.Value.Artwork.ExtractScriptPath;

    protected override IResource ToResource(string resourceUrl) =>
        factory.Create(parser.Parse(resourceUrl));

}