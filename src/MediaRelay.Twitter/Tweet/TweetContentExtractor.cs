using MediaRelay.Browser;
using MediaRelay.Content.Extract;
using MediaRelay.Http;
using MediaRelay.Resource;
using MediaRelay.Twitter.Image;
using Microsoft.Extensions.Options;
namespace MediaRelay.Twitter.Tweet;


internal sealed class TweetContentExtractor(
        IBrowserService browserService,
        ImageUrl.Parser parser,
        ImageUrlResource.Factory factory,
        IUrlResourceFactory urlResourceFactory,
        IOptions<TwitterOptions> options
    ) : UrlContentExtractor<TweetSource>(browserService)
{
    protected override IEnumerable<HttpCookieOptions> GetCookies() => options.Value.Http.Cookies;
    protected override string GetScriptFilePath() => options.Value.Tweet.ExtractScriptPath;
    protected override IResource ToResource(string resourceUrl) => factory.Create(parser.Parse(resourceUrl));



    protected override async ValueTask ExtractAsync(ExtractContext context, CancellationToken cancellationToken)
    {
        if (context.ExtractorSnapshot.Resources.Length > 0) return;

        var videoResource = await GetVideoExtractResourceAsync(context.BrowserContext, context.Source, cancellationToken);
        context.Builder.AddResources(videoResource);
    }

    private async Task<IResource> GetVideoExtractResourceAsync(IBrowserContext context, TweetSource source, CancellationToken cancellationToken)
    {
        await using var videoDownlaodPage = await context.NewPageAsync();
        await videoDownlaodPage.GotoAsync(options.Value.Tweet.VideoDownloadUrl, new() { WaitUntil = WaitUntilState.DOMContentLoaded }, cancellationToken);

        var downloadUrl = await videoDownlaodPage
            .EvaluateScriptFileAsync<string>(options.Value.Tweet.VideoDownloadUrlScriptPath, source.Url.ToString(), cancellationToken);

        return urlResourceFactory.Create
        (
            ResourceId.CreateVideoId(source.Username, source.TweetId),
            new Uri(downloadUrl),
            Storage.MediaType.Mp4
        );
    }
}