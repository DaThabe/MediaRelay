using MediaRelay.Browser;
using MediaRelay.Content;
using MediaRelay.Pixiv.Image;
using MediaRelay.Source;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MediaRelay.Pixiv.Artwork;


internal sealed class ArtworkContentExtractor(
        IBrowserService browserService,
        OriginalImageUrl.Parser parser,
        OriginalImageUrlResource.Factory factory,
        IOptions<PixivOptions> options
    ) : IContentExtractor
{
    public bool CanExtract(ISource source)
    {
        return source is ArtworkSource;
    }

    public async ValueTask<IContent> ExtractAsync(ISource source, CancellationToken cancellationToken = default)
    {
        if (source is not ArtworkSource artworkSource)
            throw new NotSupportedException($"不支持的Pixiv作品来源: {source}");

        // Browser
        await using var browser = await browserService.GetSharedAsync();
        await using var context = await browser.NewContextAsync();
        await context.AddCookiesAsync(options.Value.Http.Cookies);


        // Extract
        var builder = ArtworkContent.BuilderFromSource(artworkSource);

        // Image
        var extractSnapshot = await GetExtractSnapshotAsync(context, artworkSource, cancellationToken);
        FillToBuilder(builder, extractSnapshot);

        return builder.Build();
    }

    private void FillToBuilder(ArtworkContent.Builder builder, ArtworkContentSnapshot snapshot)
    {
        builder.SetTitle(snapshot.Title)
            .SetDescription(snapshot.Describe)
            .SetAuthor(snapshot.AuthorName, snapshot.AuthorUrl)
            .SetUploadTime(snapshot.UploadAt)
            .AddTags(snapshot.Tags)
            .AddResources(snapshot.Resources.Select(url =>
            {
                var imageUrl = parser.Parse(url);
                return factory.Create(imageUrl);
            }));
    }
    private async Task<ArtworkContentSnapshot> GetExtractSnapshotAsync(IBrowserContext context, ArtworkSource source, CancellationToken cancellationToken)
    {
        await using var page = await context.NewPageAsync();
        await page.GotoAsync(
            source.Url.ToString(),
            new PageGotoOptions() { WaitUntil = WaitUntilState.DOMContentLoaded },
            cancellationToken);

        var extractResult = await page
            .EvaluateScriptFileAsync(options.Value.Artwork.ExtractScriptPath, cancellationToken: cancellationToken);

        return JsonSerializer
            .Deserialize(extractResult, ArtworkContentSnapshotJsonSerializerContext.Default.ArtworkContentSnapshot)
            ?? throw new ArgumentNullException($"未解析到Pixiv作品内容: {source}");
    }
}



internal sealed record class ArtworkContentSnapshot
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
[JsonSerializable(typeof(ArtworkContentSnapshot))]
internal partial class ArtworkContentSnapshotJsonSerializerContext : JsonSerializerContext;