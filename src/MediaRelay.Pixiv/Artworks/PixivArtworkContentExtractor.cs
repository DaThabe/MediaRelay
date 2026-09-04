using MediaRelay.Content;
using MediaRelay.Playwright;
using MediaRelay.Source;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MediaRelay.Pixiv.Artworks;


internal sealed class PixivArtworkContentExtractor(
        IBrowserService browserService,
        IPixivDownloader pixivDownloader,
        IOptions<PixivOptions> options,
        ILogger<PixivArtworkContentExtractor> logger
    ) : BrowserContentScriptExtractor(browserService, logger)
{
    public override bool CanExtract(ISource source)
    {
        return source is PixivArtworkSource;
    }

    protected override void OnNavigating(PageGotoOptions options)
    {
        options.Timeout = 12000;
        options.WaitUntil = WaitUntilState.DOMContentLoaded;
    }
    protected override void OnContextCreating(BrowserNewContextOptions options)
    {
        //options.BypassCSP = true;
    }
    protected override async ValueTask OnContextCreated(IBrowserContext context)
    {
        await context.AddCookiesAsync(options.Value.Cookies);
    }

    protected override async ValueTask<string> LoadScriptAsync(CancellationToken cancellationToken)
    {
        return await File.ReadAllTextAsync(options.Value.ExtractScriptPath, cancellationToken);
    }

    protected override IContent ParseScriptResult(IWebPageSource webPageSource, string scriptResult)
    {
        if (webPageSource is not PixivArtworkSource source)
            throw new NotSupportedException("不是有效的Pixiv来源");

        var snapshot = JsonSerializer.Deserialize(scriptResult, SnapshotSerializerContext.Default.Snapshot);

        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentOutOfRangeException.ThrowIfZero(snapshot.Resources.Count);


        var builder = PixivArtworkContent.BuilderFromArtworkId(webPageSource.Url, source.ArtworkId)
            .SetTitle(snapshot.Title)
            .SetDescription(snapshot.Describe)
            .SetUploadTime(snapshot.UploadAt)
            .AddTags(snapshot.Tags);

        var authorUrl = new Uri(new Uri(options.Value.BaseUrl), snapshot.AuthorUrl).ToString();
        builder.SetAuthor(snapshot.AuthorName, authorUrl);

        foreach (var i in snapshot.Resources)
        {
            var resource = PixivImageUrlResource.CreateFromUrl(i, pixivDownloader);
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