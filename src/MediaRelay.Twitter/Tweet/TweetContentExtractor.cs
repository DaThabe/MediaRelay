using MediaRelay.Browser;
using MediaRelay.Content.Extract;
using MediaRelay.Http;
using MediaRelay.Resource;
using MediaRelay.Twitter.Image;
using MediaRelay.Twitter.Video;
using Microsoft.Extensions.Options;
namespace MediaRelay.Twitter.Tweet;


internal sealed class TweetContentExtractor(
        IBrowserService browserService,
        ImageUrl.Parser parser,
        ImageUrlResource.Factory factory,
        ITwitterVideoResourceFactory videoResourceFactory,
        IOptions<TwitterOptions> options
    ) : UrlSourceContentExtractor<TweetSource>(browserService)
{
    protected override IEnumerable<HttpCookieOptions> GetCookies() =>
        options.Value.Http.Cookies;

    protected override string GetScriptFilePath() =>
        options.Value.Tweet.ExtractScriptPath;

    protected override IResource ToResource(string resourceUrl) =>
        factory.Create(parser.Parse(resourceUrl));


    protected override async ValueTask ExtractAsync(ExtractContext context, CancellationToken cancellationToken)
    {
        if (context.ExtractorSnapshot.Resources.Length > 0) return;

        var videoResource = await videoResourceFactory.CreateAsync(context.Source, cancellationToken);
        context.Builder.AddResources(videoResource);
    }
}