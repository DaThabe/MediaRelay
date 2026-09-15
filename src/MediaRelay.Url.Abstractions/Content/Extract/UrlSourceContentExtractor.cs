using MediaRelay.Browser;
using MediaRelay.Content.Builder;
using MediaRelay.Content.Snapshot;
using MediaRelay.Resource;
using MediaRelay.Serialization;
using MediaRelay.Source;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MediaRelay.Content.Extract;


/// <summary>
/// 自定义网址来源的网址内容提取器
/// </summary>
/// <typeparam name="TUrlSource">网址来源类型</typeparam>
public abstract class UrlSourceContentExtractor<TUrlSource> : UrlContentExtractor<TUrlSource, DefaultUrlSnapshot>
    where TUrlSource : IUrlSource
{
    protected override ISerializer<DefaultUrlSnapshot> SnapshotSerializer { get; }
    protected abstract UrlResourceParserHandler UrlResourceParser { get; }


    protected UrlSourceContentExtractor(ILogger logger) : base(logger)
    {
        SnapshotSerializer = ServiceProvider
            .GetRequiredService<IJsonSerializerFactory>()
            .Create(DefaultUrlSnapshotJsonSerializerContext.Default.DefaultUrlSnapshot);
    }


    protected override async ValueTask<IUrlContent> ExtractAsync(ExtractContext extractContext, CancellationToken cancellationToken)
    {
        var builder = new DefaultUrlContentBuilder(ContentId.Create(extractContext.Source.Id.ToString()), extractContext.Source);
        IUrlSnapshot urlSnapshot = extractContext.ContentSnapshot;

        builder.AddResources(urlSnapshot.Resources.Select(x => UrlResourceParser(x)));
        builder.MetadataBuilder.FromSnapshot(urlSnapshot);

        var builderContext = new BuildContext()
        {
            Source = extractContext.Source,
            PageSession = extractContext.PageSession,
            ContentSnapshot = extractContext.ContentSnapshot,
            ContentBuilder = builder,
        };

        await ExtractAsync(builderContext, cancellationToken);
        return builderContext.ContentBuilder.Build();
    }

    protected virtual ValueTask ExtractAsync(BuildContext builderContext, CancellationToken cancellationToken) =>
        ValueTask.CompletedTask;


    protected class BuildContext
    {
        public required TUrlSource Source { get; init; }
        public required IPageSession PageSession { get; init; }
        public required IUrlSnapshot ContentSnapshot { get; init; }
        public required DefaultUrlContentBuilder ContentBuilder { get; init; }
    }

    protected delegate IUrlResource UrlResourceParserHandler(Uri url);
}