using MediaRelay.Browser;
using MediaRelay.Source.Url;
using Microsoft.Extensions.Logging;

namespace MediaRelay.Content;

public abstract class BrowserContentScriptExtractor(
        IBrowserService browserService,
        ILogger logger
    ) : BrowserContentExtractor(browserService, logger)
{
    private readonly ILogger _logger = logger;

    protected override async ValueTask<IContent> ExtractAsync(
        ExtractionContext context,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("开始执行脚本");

        // 执行脚本
        var scriptResult = await ExecuteScriptAsync(context.Page, cancellationToken);
        _logger.LogInformation("脚本执行完成");

        // 解析脚本结果
        var content = ParseScriptResult(context.Source, scriptResult);

        using var _ = _logger.BeginScope(new
        {
            ContentId = content.Id,
            ResourceIds = content.MediaResources.Select(x => x.Id).ToArray()
        });
        _logger.LogInformation("脚本结果解析完成");

        return content;
    }


    /// <summary>
    /// 加载脚本
    /// </summary>
    protected abstract ValueTask<string> LoadScriptAsync(CancellationToken cancellationToken);
    /// <summary>
    /// 脚本结果解析
    /// </summary>
    protected abstract IContent ParseScriptResult(IUrlSource webPageSource, string scriptResult);


    private async Task<string> ExecuteScriptAsync(IPage page, CancellationToken cancellationToken)
    {
        var script = await LoadScriptAsync(cancellationToken);
        return await page.EvaluateAsync<string>(script);
    }
}