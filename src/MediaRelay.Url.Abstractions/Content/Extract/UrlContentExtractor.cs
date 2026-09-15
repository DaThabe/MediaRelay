using MediaRelay.Browser;
using MediaRelay.Content.Snapshot;
using MediaRelay.Http;
using MediaRelay.Serialization;
using MediaRelay.Source;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MediaRelay.Content.Extract;



/// <summary>
/// 从网址来源运行自动浏览器页面执行脚本后抓取内容快照
/// </summary>
/// <typeparam name="TSource">网址来源类型</typeparam>
/// <typeparam name="TContentExtractorSnapshot">网址内容提取快照类型</typeparam>
public abstract class UrlContentExtractor<TSource, TContentExtractorSnapshot> : IUrlContentExtractor
    where TSource : IUrlSource
    where TContentExtractorSnapshot : IUrlSnapshot
{
    private readonly ILogger _logger;

    protected abstract IServiceProvider ServiceProvider { get; }
    protected virtual IPageSessionFactory PageSessionFactory { get; }
    protected virtual IUrlContentRepository UrlContentRepository { get; }


    protected abstract ISerializer<TContentExtractorSnapshot> SnapshotSerializer { get; }
    protected abstract IReadOnlySet<HttpCookieOptions> Cookies { get; }
    protected abstract string ScriptFilePath { get; }


    protected UrlContentExtractor(ILogger logger)
    {
        _logger = logger;
        PageSessionFactory = ServiceProvider.GetRequiredService<IPageSessionFactory>();
        UrlContentRepository = ServiceProvider.GetRequiredService<IUrlContentRepository>();
    }

    public virtual bool CanExtract(IUrlSource source) =>
        source is TSource;

    public virtual async ValueTask<IUrlContent> ExtractAsync(IUrlSource source, CancellationToken cancellationToken = default)
    {
        if (source is not TSource targetSource)
            throw new NotSupportedException($"不支持的网址来源: {source}");

        using var _ = _logger.BeginScope("SourceId", source.Id);

        // 使用缓存
        var content = await UrlContentRepository.FindAsync(source, cancellationToken);
        if (content is not null)
        {
            _logger.LogInformation("已使用缓存内容");
            return content;
        }

        // Goto
        await using var pageSession = await PageSessionFactory.CreateAsync();
        await pageSession.Context.AddCookiesAsync(Cookies);
        await pageSession.GotoAsync(source.Url.ToString(), cancellationToken: cancellationToken);

        // Extract
        var snapshot = await pageSession.EvaluateScriptFileAsync(ScriptFilePath, null, SnapshotSerializer, cancellationToken);

        // Context
        var context = new ExtractContext()
        {
            Source = targetSource,
            PageSession = pageSession,
            ContentSnapshot = snapshot
        };

        // 缓存
        content = await ExtractAsync(context, cancellationToken);
        await UrlContentRepository.AddAsync(content, cancellationToken);
        _logger.LogInformation("已经缓存内容");

        return content;
    }

    protected abstract ValueTask<IUrlContent> ExtractAsync(ExtractContext extractContext, CancellationToken cancellationToken);


    protected readonly struct ExtractContext
    {
        public required TSource Source { get; init; }
        public required IPageSession PageSession { get; init; }
        public required TContentExtractorSnapshot ContentSnapshot { get; init; }
    }
}