using MediaRelay.Browser;
using MediaRelay.Content;
using MediaRelay.Http;
using MediaRelay.Pixiv.Image;
using MediaRelay.Source;
using MediaRelay.Source.Url;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MediaRelay.Pixiv.Artwork;


internal sealed class ArtworkContentExtractor(
        IBrowserService browserService,
        IOptions<HttpOptions> httpOptions,
        IOptions<PixivHttpOptions> pixivHttpOptions,
        IOptions<PixivArtworkOptions> artworkOptions,
        OriginalImageUrl.Parser parser,
        OriginalImageUrlResource.Factory factory,
        ILogger<ArtworkContentExtractor> logger
    ) : BrowserContentScriptExtractor(browserService, logger)
{
    public override bool CanExtract(ISource source)
    {
        return source is ArtworkSource;
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
        await context.AddCookiesAsync(pixivHttpOptions.Value.Cookies);
    }

    protected override async ValueTask<string> LoadScriptAsync(CancellationToken cancellationToken)
    {
        return await File.ReadAllTextAsync(artworkOptions.Value.ExtractScriptPath, cancellationToken);
    }

    protected override IContent ParseScriptResult(IUrlSource webPageSource, string scriptResult)
    {
        if (webPageSource is not ArtworkSource source)
            throw new NotSupportedException($"不是有效的Pixiv作品来源: {webPageSource}");

        var snapshot = JsonSerializer.Deserialize(scriptResult, SnapshotSerializerContext.Default.Snapshot)
          ?? throw new ArgumentNullException($"未解析到Pixiv作品内容: {source}");

        if (snapshot.Resources.Count <= 0) throw new ArgumentException($"Pixiv作品内容解析中不包含媒体资源: {source}");


        var builder = ArtworkContent.BuilderFromSource(source)
            .SetTitle(snapshot.Title)
            .SetDescription(snapshot.Describe)
            .SetUploadTime(snapshot.UploadAt)
            .AddTags(snapshot.Tags);

        var authorUrl = new Uri(new Uri(pixivHttpOptions.Value.BaseUrl), snapshot.AuthorUrl).ToString();
        builder.SetAuthor(snapshot.AuthorName, authorUrl);

        foreach (var resourceUrl in snapshot.Resources)
        {
            var url = parser.Parse(resourceUrl);
            var resource = factory.Create(url);

            builder.AddResource(resource);
        }

        return builder.Build();
    }
}

internal sealed record class Snapshot
{
    public required HashSet<string> Resources { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Describe { get; init; } = string.Empty;
    public HashSet<string> Tags { get; init; } = [];
    public required DateTimeOffset UploadAt { get; init; }
    public required string AuthorName { get; init; }
    public required string AuthorUrl { get; init; }
}

[JsonSourceGenerationOptions(
    // 忽略大小写，允许驼峰和帕斯卡命名
    PropertyNameCaseInsensitive = true,
    // 格式化输出
    WriteIndented = true
)]
[JsonSerializable(typeof(Snapshot))]
internal partial class SnapshotSerializerContext : JsonSerializerContext;