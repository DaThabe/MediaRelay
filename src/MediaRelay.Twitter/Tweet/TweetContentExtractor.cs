using MediaRelay.Browser;
using MediaRelay.Content.Extract;
using MediaRelay.Http;
using MediaRelay.Resource;
using MediaRelay.Twitter.Image;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
namespace MediaRelay.Twitter.Tweet;


internal sealed class TweetContentExtractor(
        IBrowserService browserService,
        ImageUrl.Parser parser,
        ImageUrlResource.Factory factory,
        IUrlResourceFactory urlResourceFactory,
        IOptions<TwitterOptions> options
    ) : UrlContentExtractor<TweetSource, TweetContentSnapshot, TweetContent.Builder, TweetContent>(browserService)
{
    protected override IEnumerable<HttpCookieOptions> GetCookies() => options.Value.Http.Cookies;
    protected override string GetScriptFilePath() => options.Value.Tweet.ExtractScriptPath;
    protected override JsonTypeInfo<TweetContentSnapshot> GetScriptResultJsonTypeInfo() => TweetContentSnapshotJsonSerializerContext.Default.TweetContentSnapshot;
    protected override TweetContent.Builder CreateContentBuilder(TweetSource source) => TweetContent.BuilderFromSource(source);
    protected override IResource ToResource(string resourceUrl) => factory.Create(parser.Parse(resourceUrl));



    protected override async ValueTask ExtractAsync(IExtractContext context, CancellationToken cancellationToken)
    {
        if (context.ExtractorSnapshot.Resources.Count > 0) return;

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



internal sealed record class TweetContentSnapshot : IUrlExtractorSnapshot
{
    public required HashSet<string> Resources { get; init; }
    public string Content { get; init; } = string.Empty;
    public required DateTimeOffset UploadAt { get; init; }
    public required string AuthorName { get; init; }
    public required string AuthorUrl { get; init; }
    public HashSet<string> Tags { get; init; } = [];


    [JsonIgnore] DateTimeOffset? IUrlExtractorSnapshot.UploadAt => UploadAt;
    [JsonIgnore] IReadOnlySet<string> IUrlExtractorSnapshot.Resources => Resources;
    [JsonIgnore] string? IUrlExtractorSnapshot.Title => null;
    [JsonIgnore] IReadOnlySet<string> IUrlExtractorSnapshot.Tags => Tags;
}

[JsonSourceGenerationOptions(
    // 忽略大小写，允许驼峰和帕斯卡命名
    PropertyNameCaseInsensitive = true,
    // 格式化输出
    WriteIndented = true
)]
[JsonSerializable(typeof(TweetContentSnapshot))]
internal partial class TweetContentSnapshotJsonSerializerContext : JsonSerializerContext;