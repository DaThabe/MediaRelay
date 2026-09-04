using MediaRelay.Content;
using MediaRelay.Publish;
using MediaRelay.Source;
using Microsoft.Extensions.Logging;

namespace MediaRelay;


internal sealed partial class MediaRelay(
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
        LogBegin(source);

        var content = await extractorSelector
            .Select(source)
            .ExtractAsync(source, cancellationToken);

        var publishContent = await converterSelector
            .Select(content)
            .ConvertAsync(content, cancellationToken);

        await publishOrchestrator
            .PublishAsync(publishContent, cancellationToken);

        LogFinish(source);
    }


    [LoggerMessage(Level = LogLevel.Information, Message = "开始处理 < 来源 [{Source}]")]
    private partial void LogBegin(ISource source);


    [LoggerMessage(Level = LogLevel.Information, Message = "处理完成 < 来源 [{Source}]")]
    private partial void LogFinish(ISource source);
}