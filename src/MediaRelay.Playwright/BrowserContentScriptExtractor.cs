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
        LogBeginExecuteScript();

        // 执行脚本
        var scriptResult = await ExecuteScriptAsync(context.Page, cancellationToken);
        LoScriptExecuteComplete();

        // 解析脚本结果
        var content =  ParseScriptResult(context.Source, scriptResult);

        using var _ = _logger.BeginScope(new
        {
            ContentId = content.Id,
            ResourceIds = content.MediaResources.Select(x => x.Id).ToArray()
        });

        LogParsedContent();
        return content;
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


    [LoggerMessage(Level = LogLevel.Information, Message = "开始执行脚本")]
    private partial void LogBeginExecuteScript();

    [LoggerMessage(Level = LogLevel.Information, Message = "脚本执行完毕")]
    private partial void LoScriptExecuteComplete();

    [LoggerMessage(Level = LogLevel.Information, Message = "解析完成")]
    private partial void LogParsedContent();
}