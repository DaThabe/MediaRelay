using MediaRelay.Browser;
using MediaRelay.Http;
using MediaRelay.Resource;
using MediaRelay.Source;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace MediaRelay.Content.Extract;


/// <summary>
/// 自定义网址内容提取器
/// </summary>
/// <typeparam name="TUrlSource">网址内容类型</typeparam>
/// <typeparam name="TUrlContentExtractorSnapshot">网址内容提取快照类型</typeparam>
/// <typeparam name="TUrlContentBuilder">网址内容构建器类型</typeparam>
/// <typeparam name="TUrlContent">网址内容类型</typeparam>
public abstract class UrlContentExtractor<TUrlSource, TUrlContentExtractorSnapshot, TUrlContentBuilder, TUrlContent>(IBrowserService browserService) : IContentExtractor
    where TUrlSource : IUrlSource
    where TUrlContentExtractorSnapshot : IUrlContentExtractSnapshot
    where TUrlContentBuilder : IUrlContentBuilder<TUrlContentBuilder, TUrlContent>
    where TUrlContent : IUrlContent
{
    public virtual bool CanExtract(ISource source) =>
        source is TUrlSource;
    public virtual async ValueTask<IContent> ExtractAsync(ISource source, CancellationToken cancellationToken = default)
    {
        if (source is not TUrlSource targetSource)
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
        var scriptResultJsonTypeInfo = GetScriptResultJsonTypeInfo();
        var scriptResultString = await page.EvaluateScriptFileAsync<string>(scriptFilePath, null, cancellationToken);
        var snapshot = JsonSerializer.Deserialize(scriptResultString, scriptResultJsonTypeInfo);

        // Builder
        ArgumentNullException.ThrowIfNull(snapshot);
        var builder = CreateContentBuilder(targetSource)
            .SetAuthor(snapshot.AuthorName, snapshot.AuthorUrl)
            .SetTitle(snapshot.Title)
            .SetContent(snapshot.Content)
            .AddTags(snapshot.Tags)
            .AddResources(snapshot.Resources.Select(ToResource));

        // Context
        var extractContext = new ExtractContext()
        {
            Source = targetSource,
            Browser = browser,
            BrowserContext = context,
            Page = page,
            ScriptFilePath = scriptFilePath,
            ScriptResultJsonTypeInfo = scriptResultJsonTypeInfo,
            ExtractorSnapshot = snapshot,
            Builder = builder
        };
        await ExtractAsync(extractContext, cancellationToken);

        return extractContext.Builder.Build();
    }

    // 脚本文件路径
    protected abstract string GetScriptFilePath();
    // 脚本结果Json类型信息
    protected abstract JsonTypeInfo<TUrlContentExtractorSnapshot> GetScriptResultJsonTypeInfo();
    protected abstract TUrlContentBuilder CreateContentBuilder(TUrlSource source);
    // 资源转换
    protected abstract IResource ToResource(string resourceUrl);
    // 获取Cookie
    protected abstract IEnumerable<HttpCookieOptions> GetCookies();


    // 提取任务
    protected virtual ValueTask ExtractAsync(ExtractContext context, CancellationToken cancellationToken) =>
        ValueTask.CompletedTask;


    protected sealed class ExtractContext
    {
        public required TUrlSource Source { get; init; }
        public required IBrowser Browser { get; init; }
        public required IBrowserContext BrowserContext { get; init; }
        public required IPage Page { get; init; }

        public required string ScriptFilePath { get; init; }
        public required TUrlContentExtractorSnapshot ExtractorSnapshot { get; init; }
        public required JsonTypeInfo<TUrlContentExtractorSnapshot> ScriptResultJsonTypeInfo { get; init; }

        public required TUrlContentBuilder Builder { get; init; }
    }
}