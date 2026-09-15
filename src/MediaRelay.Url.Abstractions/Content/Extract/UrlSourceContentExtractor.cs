using MediaRelay.Browser;
using MediaRelay.Content.Builder;
using MediaRelay.Content.Snapshot;
using MediaRelay.Resource;
using MediaRelay.Serialization;
using MediaRelay.Source;

namespace MediaRelay.Content.Extract;


/// <summary>
/// 自定义网址来源的网址内容提取器
/// </summary>
/// <typeparam name="TUrlSource">网址来源类型</typeparam>
public abstract class UrlSourceContentExtractor<TUrlSource> : UrlContentExtractor<TUrlSource, DefaultUrlSnapshot>
    where TUrlSource : IUrlSource
{
    protected abstract IJsonSerializerFactory JsonSerializerFactory { get; }
    protected override ISerializer<DefaultUrlSnapshot> SnapshotSerializer { get; }
    protected abstract UrlResourceParserHandler UrlResourceParser { get; }


    protected UrlSourceContentExtractor()
    {
        SnapshotSerializer = JsonSerializerFactory.Create(DefaultUrlSnapshotJsonSerializerContext.Default.DefaultUrlSnapshot);
    }


    protected override async ValueTask<IUrlContent> ExtractAsync(UrlContentExtractor<TUrlSource, DefaultUrlSnapshot>.Context context, CancellationToken cancellationToken)
    {
        var builder = new DefaultUrlContentBuilder(ContentId.Create(context.Source.Id.ToString()), context.Source);
        IUrlSnapshot urlSnapshot = context.ContentSnapshot;

        builder.AddResources(urlSnapshot.Resources.Select(x => UrlResourceParser(x)));
        builder.MetadataBuilder.FromSnapshot(urlSnapshot);

        var newContext = new Context()
        {
            Source = context.Source,
            PageSession = context.PageSession,
            ContentSnapshot = context.ContentSnapshot,
            ContentBuilder = builder,
        };

        await ExtractAsync(context, cancellationToken);
        return newContext.ContentBuilder.Build();
    }

    protected virtual ValueTask ExtractAsync(Context context, CancellationToken cancellationToken) =>
        ValueTask.CompletedTask;


    protected new class Context
    {
        public required TUrlSource Source { get; init; }
        public required IPageSession PageSession { get; init; }
        public required IUrlSnapshot ContentSnapshot { get; init; }
        public required DefaultUrlContentBuilder ContentBuilder { get; init; }
    }

    protected delegate IUrlResource UrlResourceParserHandler(Uri url);
}