using MediaRelay.Content.Extract;
using MediaRelay.HeyBox.Image;
using MediaRelay.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MediaRelay.HeyBox.BbsLink;


internal sealed class LinkContentExtractor(
        IServiceProvider serviceProvider,
        OriginalImageUrl.Parser parser,
        OriginalImageUrlResource.Factory factory,
        IOptions<HeyBoxOptions> options,
        ILogger<LinkContentExtractor> logger
    ) : UrlSourceContentExtractor<LinkSource>(logger)
{
    protected override IServiceProvider ServiceProvider => serviceProvider;
    protected override IReadOnlySet<HttpCookieOptions> Cookies { get; } = options.Value.Http.Cookies.ToHashSet().AsReadOnly();
    protected override string ScriptFilePath { get; } = options.Value.BbsLink.ExtractScriptPath;
    protected override UrlResourceParserHandler UrlResourceParser { get; } = url => factory.Create(parser.Parse(url));
}