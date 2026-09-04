using MediaRelay.Content;
using MediaRelay.Source;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

namespace MediaRelay.Playwright;

public abstract partial class BrowserContentScriptExtractor(
        IBrowserService browserService,
        ILogger logger
    ) : BrowserContentExtractor(browserService, logger)
{
    private readonly ILogger _logger = logger;

    protected override async ValueTask<IContent> ExtractAsync(
        ExtractionContext context,
        CancellationToken cancellationToken)
    {
        // 执行脚本
        var scriptResult = await ExecuteScriptAsync(context.Page, cancellationToken);
        // 解析脚本结果
        return ParseScriptResult(context.Source, scriptResult);
    }


    /// <summary>
    /// 加载脚本
    /// </summary>
    protected abstract ValueTask<string> LoadScriptAsync(CancellationToken cancellationToken);
    /// <summary>
    /// 脚本结果解析
    /// </summary>
    protected abstract IContent ParseScriptResult(IWebPageSource webPageSource, string scriptResult);


    private async Task<string> ExecuteScriptAsync(IPage page, CancellationToken cancellationToken)
    {
        var script = await LoadScriptAsync(cancellationToken);
        return await page.EvaluateAsync<string>(script);
    }


    [LoggerMessage(Level = LogLevel.Debug, Message = "开始提取内容")]
    private partial void LogBeginExtract();
}