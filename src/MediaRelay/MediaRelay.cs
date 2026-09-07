using MediaRelay.Content;
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
        logger.LogInformation("开始处理");

        // Extract
        var content = await extractorSelector
            .Select(source)
            .ExtractAsync(source, cancellationToken);

        using var _ = logger.BeginScope("content", content);
        logger.LogInformation("提取到内容");

        // Convert
        var publishContent = await converterSelector
            .Select(content)
            .ConvertAsync(content, cancellationToken);

        using var __ = logger.BeginScope("PublishContent", publishContent);
        logger.LogInformation("提取到内容");

        // Publish
        await publishOrchestrator
            .PublishAsync(publishContent, cancellationToken);
    }
}