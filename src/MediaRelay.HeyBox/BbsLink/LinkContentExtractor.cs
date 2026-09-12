using MediaRelay.Browser;
using MediaRelay.Content.Extract;
using MediaRelay.HeyBox.Image;
using MediaRelay.Http;
using MediaRelay.Resource;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace MediaRelay.HeyBox.BbsLink;


internal sealed class LinkContentExtractor(
        IBrowserService browserService,
        OriginalImageUrl.Parser parser,
        OriginalImageUrlResource.Factory factory,
        IOptions<HeyBoxOptions> options
    ) : UrlContentExtractor<LinkSource, LinkContentExtractorSnapshot, LinkContent.Builder, LinkContent>(browserService)
{
    protected override IEnumerable<HttpCookieOptions> GetCookies() =>
        options.Value.Http.Cookies;

    protected override LinkContent.Builder CreateContentBuilder(LinkSource source) =>
        LinkContent.BuilderFromSource(source);

    protected override string GetScriptFilePath() =>
        options.Value.BbsLink.ExtractScriptPath;

    protected override JsonTypeInfo<LinkContentExtractorSnapshot> GetScriptResultJsonTypeInfo() =>
        LinkContentSnapshotJsonSerializerContext.Default.LinkContentExtractorSnapshot;

    protected override IResource ToResource(string resourceUrl) =>
        factory.Create(parser.Parse(resourceUrl));

}


internal sealed record class LinkContentExtractorSnapshot : IUrlExtractorSnapshot
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
[JsonSerializable(typeof(LinkContentExtractorSnapshot))]
internal partial class LinkContentSnapshotJsonSerializerContext : JsonSerializerContext;