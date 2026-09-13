using MediaRelay.Browser;
using MediaRelay.Content.Builder;
using MediaRelay.Content.Snapshot;
using MediaRelay.Http;
using MediaRelay.Metadata;
using MediaRelay.Resource;
using MediaRelay.Source;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace MediaRelay.Content.Extract;


/// <summary>
/// 自定义网址内容提取器
/// </summary>
/// <typeparam name="TSource">网址内容类型</typeparam>
/// <typeparam name="TContentExtractorSnapshot">网址内容提取快照类型</typeparam>
/// <typeparam name="TContentBuilder">网址内容构建器类型</typeparam>
/// <typeparam name="TContent">网址内容类型</typeparam>
public abstract class UrlContentExtractor<TSource, TContentExtractorSnapshot, TContentBuilder, TContent, TMetadataBuilder, TMetadata>(IBrowserService browserService) : IContentExtractor
    where TSource : IUrlSource
    where TContentExtractorSnapshot : IUrlSnapshot
    where TContentBuilder : IUrlContentBuilder<TContentBuilder, TContent, TMetadataBuilder, TMetadata>
    where TContent : IUrlContent
    where TMetadataBuilder : IUrlMetadataBuilder<TMetadataBuilder, TMetadata, TContentBuilder, TContent>
    where TMetadata : IUrlMetadata
{
    public virtual bool CanExtract(ISource source) =>
        source is TSource;
    public virtual async ValueTask<IContent> ExtractAsync(ISource source, CancellationToken cancellationToken = default)
    {
        if (source is not TSource targetSource)
            throw new NotSupportedException($"不支持的推文来源: {source}");

        // Browser
        await using var browser = await browserService.GetSharedAsync();
        // Context
        await using var context = await browser.NewContextAsync();
        await context.AddCookiesAsync(GetCookies());
        // Page
        await using var page = await context.NewPageAsync();
        // Goto
        var gotoOptions = new PageGotoOptions() { WaitUntil = WaitUntilState.DOMContentLoaded };
        await page.GotoAsync(targetSource.Url.ToString(), gotoOptions, cancellationToken);

        // Extract
        var scriptFilePath = GetScriptFilePath();
        var snapshotJsonTypeInfo = GetSnapshotJsonTypeInfo();
        var scriptResultString = await page.EvaluateScriptFileAsync<string>(scriptFilePath, null, cancellationToken);
        var snapshot = JsonSerializer.Deserialize(scriptResultString, snapshotJsonTypeInfo);

        // Builder
        ArgumentNullException.ThrowIfNull(snapshot);
        var builder = CreateContentBuilder(targetSource)
            .MetadataBuilder
            .FromMetadata(snapshot.Metadata)
            .ContentBuilder
            .AddResources(snapshot.Resources.Select(ToResource));

        // Context
        var extractContext = new ExtractContext()
        {
            Source = targetSource,
            Browser = browser,
            BrowserContext = context,
            Page = page,
            ScriptFilePath = scriptFilePath,
            SnapshotJsonTypeInfo = snapshotJsonTypeInfo,
            ExtractorSnapshot = snapshot,
            Builder = builder
        };
        await ExtractAsync(extractContext, cancellationToken);

        return extractContext.Builder.Build();
    }

    // 脚本文件路径
    protected abstract string GetScriptFilePath();
    // 脚本结果Json类型信息
    protected abstract JsonTypeInfo<TContentExtractorSnapshot> GetSnapshotJsonTypeInfo();
    protected abstract TContentBuilder CreateContentBuilder(TSource source);
    // 资源转换
    protected abstract IResource ToResource(string resourceUrl);
    // 获取Cookie
    protected abstract IEnumerable<HttpCookieOptions> GetCookies();


    // 提取任务
    protected virtual ValueTask ExtractAsync(ExtractContext context, CancellationToken cancellationToken) =>
        ValueTask.CompletedTask;


    protected sealed class ExtractContext
    {
        public required TSource Source { get; init; }
        public required IBrowser Browser { get; init; }
        public required IBrowserContext BrowserContext { get; init; }
        public required IPage Page { get; init; }

        public required string ScriptFilePath { get; init; }
        public required TContentExtractorSnapshot ExtractorSnapshot { get; init; }
        public required JsonTypeInfo<TContentExtractorSnapshot> SnapshotJsonTypeInfo { get; init; }

        public required TContentBuilder Builder { get; init; }
    }
}