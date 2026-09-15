using MediaRelay.Source;

namespace MediaRelay.Content.Extract;


public interface IUrlContentExtractor : IContentExtractor
{
    bool CanExtract(IUrlSource source);
    ValueTask<IUrlContent> ExtractAsync(IUrlSource source, CancellationToken cancellationToken = default);



    bool IContentExtractor.CanExtract(ISource source)
    {
        return source is IUrlSource urlSource && CanExtract(urlSource);
    }
    async ValueTask<IContent> IContentExtractor.ExtractAsync(ISource source, CancellationToken cancellationToken)
    {
        if (source is not IUrlSource urlSource)
            throw new NotSupportedException($"仅支持网址来源, 当前: {source}");

        return await ExtractAsync(urlSource, cancellationToken);
    }
}
