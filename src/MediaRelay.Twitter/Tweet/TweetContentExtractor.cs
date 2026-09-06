using MediaRelay.Browser;
using MediaRelay.Content;
using MediaRelay.Http;
using MediaRelay.Source;
using MediaRelay.Source.Url;
using MediaRelay.Twitter.Image;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace MediaRelay.Twitter.Tweet;


internal sealed class TweetContentExtractor(
        IBrowserService browserService,
        IOptions<HttpOptions> httpOptions,
        IOptions<TwitterHttpOptions> twitterHttpOptions,
        IOptions<TwitterTweetOptions> tweetOptions,
        ImageUrl.Parser parser,
        ImageUrlResource.Factory factory,
        ILogger<TweetContentExtractor> logger
    ) : BrowserContentScriptExtractor(browserService, logger)
{
    public override bool CanExtract(ISource source)
    {
        return source is TweetSource;
    }

    protected override void OnNavigating(PageGotoOptions options)
    {
        options.Timeout = httpOptions.Value.Timeout;
        options.WaitUntil = WaitUntilState.DOMContentLoaded;
    }
    protected override void OnContextCreating(BrowserNewContextOptions options)
    {
        //options.BypassCSP = true;
    }
    protected override async ValueTask OnContextCreated(IBrowserContext context)
    {
        await context.AddCookiesAsync(httpOptions.Value.Cookies);
        await context.AddCookiesAsync(twitterHttpOptions.Value.Cookies);
    }

    protected override async ValueTask<string> LoadScriptAsync(CancellationToken cancellationToken)
    {
        return await File.ReadAllTextAsync(tweetOptions.Value.ExtractScriptPath, cancellationToken);
    }

    protected override IContent ParseScriptResult(IUrlSource webPageSource, string scriptResult)
    {
        if (webPageSource is not TweetSource source)
            throw new NotSupportedException($"不支持的推文来源: {webPageSource}");

        var snapshot = JsonSerializer.Deserialize(scriptResult, SnapshotSerializerContext.Default.Snapshot) 
            ?? throw new ArgumentNullException($"未解析到推文内容: {source}");

        if (snapshot.Resources.Count <= 0) throw new ArgumentException($"推文解析内容中不包含媒体资源: {source}");

        return TweetContent.BuilderFromSource(source)
            .SetContent(snapshot.Content)
            .SetAuthor(snapshot.AuthorName, snapshot.AuthorUrl)
            .SetUploadTime(snapshot.UploadAt)
            .AddResources(snapshot.Resources.Select(url =>
            {
                var imageUrl = parser.Parse(url, ImageSize.Original);
                return factory.Create(imageUrl);
            }))
            .Build();
    }
}

internal sealed record class Snapshot
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
[JsonSerializable(typeof(Snapshot))]
internal partial class SnapshotSerializerContext : JsonSerializerContext;