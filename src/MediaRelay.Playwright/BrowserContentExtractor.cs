using MediaRelay.Content;
using MediaRelay.Source;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

namespace MediaRelay.Playwright;


public abstract partial class BrowserContentExtractor(
        IBrowserService browserService,
        ILogger logger
    ) : IContentExtractor
{
    /// <summary>
    /// 判断内容是否可以提取
    /// </summary>
    public abstract bool CanExtract(ISource source);

    /// <summary>
    /// 开始提取内容
    /// </summary>
    /// <exception cref="NotSupportedException"></exception>
    public async ValueTask<IContent> ExtractAsync(
        ISource source,
        CancellationToken cancellationToken)
    {
        if (source is not IWebPageSource webPageSource)
            throw new NotSupportedException("不是有效的网页来源");

        using var _ = logger.Scope()
            .Add("Url", webPageSource.Url)
            .Begin();

        logger.LogInformation("开始提取内容");

        // 浏览器
        await using var browser = await browserService.GetSharedAsync();
        // 浏览器上下文
        await using var browserContext = await CreateBrowserContext(browser);
        // 浏览器页面
        await using var page = await CreateBrowserPage(browserContext);

        // 页面跳转
        await GotoPageAsync(page, webPageSource.Url);

        // 提取
        var context = new ExtractionContext()
        {
            Browser = browser,
            BrowserContext = browserContext,
            Page = page,
            Source = webPageSource
        };

        var content = await ExtractAsync(context, cancellationToken);

        using var __ = logger.Scope()
            .Add("Content", content)
            .Begin();
        logger.LogInformation("内容提取完成");

        return content;
    }

    /// <summary>
    /// 浏览器上下问正在创建
    /// </summary>
    protected virtual void OnContextCreating(BrowserNewContextOptions options)
    {
        options.ViewportSize = new ViewportSize { Width = 1920, Height = 1080 };
    }
    /// <summary>
    /// 浏览器上下问已创建
    /// </summary>
    protected virtual ValueTask OnContextCreated(IBrowserContext context)
    {
        return ValueTask.CompletedTask;
    }
    /// <summary>
    /// 导航中
    /// </summary>
    protected virtual void OnNavigating(PageGotoOptions options)
    {
        options.Timeout = 30000;
        options.WaitUntil = WaitUntilState.DOMContentLoaded;
    }


    private async Task GotoPageAsync(IPage page, Uri url)
    {
        var options = new PageGotoOptions();
        OnNavigating(options);

        logger.LogInformation("正在跳转页面");
        _ = await page.GotoAsync(url.ToString(), options);
    }

    private async Task<IBrowserContext> CreateBrowserContext(IBrowser browser)
    {
        var browserNewContextOptions = new BrowserNewContextOptions();
        OnContextCreating(browserNewContextOptions);

        var context = await browser.NewContextAsync(browserNewContextOptions);
        await OnContextCreated(context);
        logger.LogDebug("已创建浏览器上下文");

        return context;
    }

    private async Task<IPage> CreateBrowserPage(IBrowserContext context)
    {
        var page = await context.NewPageAsync();
        logger.LogDebug("已新建浏览器页面");

        return page;
    }
}



public abstract partial class BrowserContentExtractor
{
    protected abstract ValueTask<IContent> ExtractAsync(
        ExtractionContext context,
        CancellationToken cancellationToken);


    protected readonly struct ExtractionContext
    {
        public required IBrowser Browser { get; init; }
        public required IBrowserContext BrowserContext { get; init; }
        public required IPage Page { get; init; }
        public required IWebPageSource Source { get; init; }
    }
}