using MediaRelay.Browser;
using MediaRelay.Content;
using MediaRelay.Resources;
using MediaRelay.Resources.Url;
using MediaRelay.Source;
using MediaRelay.Twitter.Image;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace MediaRelay.Twitter.Tweet;


//internal sealed class TweetContentExtractor(
//        IBrowserService browserService,
//        IOptions<HttpOptions> httpOptions,
//        IOptions<TwitterHttpOptions> twitterHttpOptions,
//        IOptions<TwitterTweetOptions> tweetOptions,
//        IVideoDownloader videoDownloader,
//        ImageUrl.Parser parser,
//        ImageUrlResource.Factory factory,
//        ILogger<TweetContentExtractor> logger
//    ) : BrowserContentScriptExtractor(browserService, logger)
//{
//    public override bool CanExtract(ISource source)
//    {
//        return source is TweetSource;
//    }

//    protected override void OnNavigating(PageGotoOptions options)
//    {
//        options.Timeout = httpOptions.Value.Timeout;
//        options.WaitUntil = WaitUntilState.DOMContentLoaded;
//    }
//    protected override void OnContextCreating(BrowserNewContextOptions options)
//    {
//        //options.BypassCSP = true;
//    }
//    protected override async ValueTask OnContextCreated(IBrowserContext context)
//    {
//        await context.AddCookiesAsync(httpOptions.Value.Cookies);
//        await context.AddCookiesAsync(twitterHttpOptions.Value.Cookies);
//    }

//    protected override async ValueTask<string> LoadScriptAsync(CancellationToken cancellationToken)
//    {
//        return await File.ReadAllTextAsync(tweetOptions.Value.ExtractScriptPath, cancellationToken);
//    }

//    protected override IContent ParseScriptResult(IUrlSource webPageSource, string scriptResult)
//    {
//        if (webPageSource is not TweetSource source)
//            throw new NotSupportedException($"不支持的推文来源: {webPageSource}");

//        var snapshot = JsonSerializer.Deserialize(scriptResult, SnapshotSerializerContext.Default.Snapshot)
//            ?? throw new ArgumentNullException($"未解析到推文内容: {source}");

//        // 如果1张图像都没有试一试视频
//        if (snapshot.Resources.Count == 0)
//        {
//            throw new ArgumentException($"推文解析内容中不包含媒体资源: {source}");
//        }

//        return TweetContent.BuilderFromSource(source)
//            .SetContent(snapshot.Content)
//            .SetAuthor(snapshot.AuthorName, snapshot.AuthorUrl)
//            .SetUploadTime(snapshot.UploadAt)
//            .AddTags(snapshot.Tags)
//            .AddResources(snapshot.Resources.Select(url =>
//            {
//                var imageUrl = parser.Parse(url, ImageSize.Original);
//                return factory.Create(imageUrl);
//            }))
//            .Build();
//    }
//}


internal sealed class TweetContentExtractor(
        IBrowserService browserService,
        ImageUrl.Parser parser,
        ImageUrlResource.Factory factory,
        IUrlResourceFactory urlResourceFactory,
        IOptions<TwitterOptions> options
    ) : IContentExtractor
{
    public bool CanExtract(ISource source)
    {
        return source is TweetSource;
    }

    public async ValueTask<IContent> ExtractAsync(ISource source, CancellationToken cancellationToken = default)
    {
        if (source is not TweetSource tweetSource)
            throw new NotSupportedException($"不支持的推文来源: {source}");

        // Browser
        await using var browser = await browserService.GetSharedAsync();
        await using var context = await browser.NewContextAsync();
        await context.AddCookiesAsync(options.Value.Http.Cookies);


        // Extract
        var builder = TweetContent.BuilderFromSource(tweetSource);

        // Image
        var extractSnapshot = await GetExtractSnapshotAsync(context, tweetSource, cancellationToken);
        FillToBuilder(builder, extractSnapshot);

        // Video
        if (extractSnapshot.Resources.Count == 0)
        {
            var videoResource = await GetVideoExtractResourceAsync(context, tweetSource, cancellationToken);
            builder.AddResources(videoResource);
        }

        return builder.Build();
    }

    private void FillToBuilder(TweetContent.Builder builder, TweetContentSnapshot snapshot)
    {
        builder.SetContent(snapshot.Content)
            .SetAuthor(snapshot.AuthorName, snapshot.AuthorUrl)
            .SetUploadTime(snapshot.UploadAt)
            .AddTags(snapshot.Tags)
            .AddResources(snapshot.Resources.Select(url =>
            {
                var imageUrl = parser.Parse(url, ImageSize.Original);
                return factory.Create(imageUrl);
            }));
    }
    private async Task<TweetContentSnapshot> GetExtractSnapshotAsync(IBrowserContext context, TweetSource source, CancellationToken cancellationToken)
    {
        await using var page = await context.NewPageAsync();
        await page.GotoAsync(
            source.Url.ToString(),
            new PageGotoOptions() { WaitUntil = WaitUntilState.DOMContentLoaded },
            cancellationToken);

        var extractResult = await page
            .EvaluateScriptFileAsync(options.Value.Tweet.ExtractScriptPath, cancellationToken: cancellationToken);

        return JsonSerializer
            .Deserialize(extractResult, TweetContentSnapshotJsonSerializerContext.Default.TweetContentSnapshot)
            ?? throw new ArgumentNullException($"未解析到推文内容: {source}");
    }
    private async Task<IResource> GetVideoExtractResourceAsync(IBrowserContext context, TweetSource source, CancellationToken cancellationToken)
    {
        await using var videoDownlaodPage = await context.NewPageAsync();
        await videoDownlaodPage.GotoAsync(options.Value.Tweet.VideoDownloadUrl, new() { WaitUntil = WaitUntilState.DOMContentLoaded });

        var downloadUrl = await videoDownlaodPage
            .EvaluateScriptFileAsync(options.Value.Tweet.VideoDownloadUrlScriptPath, source.Url.ToString(), cancellationToken);

        return urlResourceFactory.Create
        (
            ResourceId.CreateVideoId(source.Username, source.TweetId),
            new Uri(downloadUrl),
            Storage.MediaType.Mp4
        );
    }
}

internal sealed record class TweetContentSnapshot
{
    public required HashSet<string> Resources { get; init; }
    public string Content { get; init; } = string.Empty;
    public required DateTimeOffset UploadAt { get; init; }
    public required string AuthorName { get; init; }
    public required string AuthorUrl { get; init; }
    public HashSet<string> Tags { get; init; } = [];
}

[JsonSourceGenerationOptions(
    // 忽略大小写，允许驼峰和帕斯卡命名
    PropertyNameCaseInsensitive = true,
    // 格式化输出
    WriteIndented = true
)]
[JsonSerializable(typeof(TweetContentSnapshot))]
internal partial class TweetContentSnapshotJsonSerializerContext : JsonSerializerContext;