using MediaRelay.Browser;
using MediaRelay.Http;
using MediaRelay.Storage;
using MediaRelay.Twitter.Tweet;
using Microsoft.Extensions.Options;

namespace MediaRelay.Twitter.Video;

internal interface IVideoDownloader
{
    ValueTask<StorageInfo> DownloadAsync(TweetSource source, CancellationToken cancellationToken);
}

internal sealed class VideoDownloader(
    IBrowserService browserService,
    IStorage storage,
    IHttpClient httpClient,
    IOptions<TwitterTweetOptions> options) : IVideoDownloader
{
    public async ValueTask<StorageInfo> DownloadAsync(TweetSource source, CancellationToken cancellationToken)
    {
        await using var context = await browserService.GetSharedContextAsync();
        await using var page = await context.NewPageAsync();
        await page.GotoAsync(options.Value.VideoDownloadUrl, cancellationToken: cancellationToken);

        var downloadUrl = await page.EvaluateScriptFileAsync<string>(options.Value.ExtractScriptPath, null, cancellationToken);

        var stream = await httpClient.GetStreamAsync(downloadUrl, cancellationToken);
        return await storage.StoreAsync(stream, MediaType.Mp4, cancellationToken);
    }
}