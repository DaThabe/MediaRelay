using MediaRelay.Browser;
using MediaRelay.Content.Extract;
using MediaRelay.Http;
using MediaRelay.Pixiv.Image;
using MediaRelay.Resources;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace MediaRelay.Pixiv.Artwork;


internal sealed class ArtworkContentExtractor(
        IBrowserService browserService,
        OriginalImageUrl.Parser parser,
        OriginalImageUrlResource.Factory factory,
        IOptions<PixivOptions> options
    ) : UrlContentExtractor<ArtworkSource, ArtworkContentSnapshot, ArtworkContent.Builder, ArtworkContent>(browserService)
{
    protected override IEnumerable<HttpCookieOptions> GetCookies() =>
        options.Value.Http.Cookies;

    protected override ArtworkContent.Builder CreateContentBuilder(ArtworkSource source) =>
        ArtworkContent.BuilderFromSource(source);

    protected override string GetScriptFilePath() =>
        options.Value.Artwork.ExtractScriptPath;

    protected override JsonTypeInfo<ArtworkContentSnapshot> GetScriptResultJsonTypeInfo() =>
        ArtworkContentSnapshotJsonSerializerContext.Default.ArtworkContentSnapshot;

    protected override IResource ToResource(string resourceUrl) =>
        factory.Create(parser.Parse(resourceUrl));

}


internal sealed record class ArtworkContentSnapshot : IUrlExtractorSnapshot
{
    public required HashSet<string> Resources { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Describe { get; init; } = string.Empty;
    public HashSet<string> Tags { get; init; } = [];
    public required DateTimeOffset UploadAt { get; init; }
    public required string AuthorName { get; init; }
    public required string AuthorUrl { get; init; }



    [JsonIgnore] IReadOnlySet<string> IUrlExtractorSnapshot.Resources => Resources;
    [JsonIgnore] string? IUrlExtractorSnapshot.Content => Describe;
    [JsonIgnore] DateTimeOffset? IUrlExtractorSnapshot.UploadAt => UploadAt;
    [JsonIgnore] IReadOnlySet<string> IUrlExtractorSnapshot.Tags => Tags;
}

[JsonSourceGenerationOptions(
    // 忽略大小写，允许驼峰和帕斯卡命名
    PropertyNameCaseInsensitive = true,
    // 格式化输出
    WriteIndented = true
)]
[JsonSerializable(typeof(ArtworkContentSnapshot))]
internal partial class ArtworkContentSnapshotJsonSerializerContext : JsonSerializerContext;