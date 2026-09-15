using MediaRelay.Browser;
using MediaRelay.Content.Snapshot;
using MediaRelay.Http;
using MediaRelay.Serializer;
using MediaRelay.Source;

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
    protected abstract IReadOnlySet<HttpCookieOptions> Cookies { get; }
    protected abstract IPageSessionFactory PageSessionFactory { get; }
    protected abstract string ScriptFilePath { get; }
    protected abstract ISerializer<TContentExtractorSnapshot> SnapshotSerializer { get; }



    public virtual bool CanExtract(IUrlSource source) =>
        source is TSource;

    public virtual async ValueTask<IUrlContent> ExtractAsync(IUrlSource source, CancellationToken cancellationToken = default)
    {
        if (source is not TSource targetSource)
            throw new NotSupportedException($"不支持的网址来源: {source}");

        // Goto
        await using var pageSession = await PageSessionFactory.CreateAsync();
        await pageSession.Context.AddCookiesAsync(Cookies);
        await pageSession.GotoAsync(source.Url.ToString(), cancellationToken: cancellationToken);

        // Extract
        var snapshot = await pageSession.EvaluateScriptFileAsync(ScriptFilePath, null, SnapshotSerializer, cancellationToken);

        // Context
        var context = new Context()
        {
            Source = targetSource,
            PageSession = pageSession,
            ContentSnapshot = snapshot
        };

        return await ExtractAsync(context, cancellationToken);
    }

    protected abstract ValueTask<IUrlContent> ExtractAsync(Context context, CancellationToken cancellationToken);


    protected readonly struct Context
    {
        public required TSource Source { get; init; }
        public required IPageSession PageSession { get; init; }
        public required TContentExtractorSnapshot ContentSnapshot { get; init; }
    }
}