using MediaRelay.Content.Extract;
using MediaRelay.Http;
using MediaRelay.Pixiv.Image;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MediaRelay.Pixiv.Artwork;


internal sealed class ArtworkContentExtractor(
        IServiceProvider serviceProvider,
        OriginalImageUrl.Parser parser,
        OriginalImageUrlResource.Factory factory,
        IOptions<PixivOptions> options,
        ILogger<ArtworkContentExtractor> logger
    ) : UrlSourceContentExtractor<ArtworkSource>(logger)
{
    protected override IServiceProvider ServiceProvider => serviceProvider;
    protected override IReadOnlySet<HttpCookieOptions> Cookies { get; } = options.Value.Http.Cookies.ToHashSet().AsReadOnly();
    protected override string ScriptFilePath { get; } = options.Value.Artwork.ExtractScriptPath;
    protected override UrlResourceParserHandler UrlResourceParser { get; } = url => factory.Create(parser.Parse(url));
}