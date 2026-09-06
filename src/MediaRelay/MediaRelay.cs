using MediaRelay.Content;
using MediaRelay.Extensions;
using MediaRelay.Publish;
using MediaRelay.Source;
using Microsoft.Extensions.Logging;

namespace MediaRelay;


internal sealed class MediaRelay(
        IContentExtractorSelector extractorSelector,
        IPublishContentConverterSelector converterSelector,
        IPublishOrchestrator publishOrchestrator,
        ILogger<MediaRelay> logger
    ) : IMediaRelay
{
    public async ValueTask HandleAsync(
        ISource source,
        CancellationToken cancellationToken = default)
    {
        using var _ = logger.Scope()
            .Add("SourceId", source.Id)
            .Begin();

        logger.LogInformation("开始处理");

        var content = await extractorSelector
            .Select(source)
            .ExtractAsync(source, cancellationToken);

        var publishContent = await converterSelector
            .Select(content)
            .ConvertAsync(content, cancellationToken);

        await publishOrchestrator
            .PublishAsync(publishContent, cancellationToken);

        logger.LogInformation("处理完成");
    }
}