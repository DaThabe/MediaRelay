using MediaRelay.Browser;
using MediaRelay.Http;
using MediaRelay.Resources;
using MediaRelay.Source;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace MediaRelay.Content.Extract;


public abstract class UrlContentExtractor<TSource, TExtractorSnapshot, TContentBuilder, TContent>(IBrowserService browserService) : IContentExtractor
    where TSource : IUrlSource
    where TExtractorSnapshot : IUrlExtractorSnapshot
    where TContentBuilder : IUrlContentBuilder<TContentBuilder, TContent>
    where TContent : IUrlContent
{
    public virtual bool CanExtract(ISource source) => source is TSource;
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
    protected abstract JsonTypeInfo<TExtractorSnapshot> GetScriptResultJsonTypeInfo();
    protected abstract TContentBuilder CreateContentBuilder(TSource source);
    // 资源转换
    protected abstract IResource ToResource(string resourceUrl);
    // 获取Cookie
    protected abstract IEnumerable<HttpCookieOptions> GetCookies();


    // 提取任务
    protected virtual ValueTask ExtractAsync(IExtractContext context, CancellationToken cancellationToken) =>
        ValueTask.CompletedTask;


    protected interface IExtractContext
    {
        TSource Source { get; }
        IBrowser Browser { get; }
        IBrowserContext BrowserContext { get; }
        IPage Page { get; }
        string ScriptFilePath { get; }
        JsonTypeInfo<TExtractorSnapshot> ScriptResultJsonTypeInfo { get; }
        TContentBuilder Builder { get; }
        TExtractorSnapshot ExtractorSnapshot { get; }
    }
    private sealed class ExtractContext : IExtractContext
    {
        public required TSource Source { get; init; }
        public required IBrowser Browser { get; init; }
        public required IBrowserContext BrowserContext { get; init; }
        public required IPage Page { get; init; }

        public required string ScriptFilePath { get; init; }
        public required TExtractorSnapshot ExtractorSnapshot { get; init; }
        public required JsonTypeInfo<TExtractorSnapshot> ScriptResultJsonTypeInfo { get; init; }

        public required TContentBuilder Builder { get; init; }
    }
}