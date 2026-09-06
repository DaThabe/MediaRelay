using MediaRelay.Extensions;
using MediaRelay.Publish;
using Microsoft.Extensions.Logging;

namespace MediaRelay.Content;


internal sealed class ContentHandlerSelector(
        IEnumerable<IPublishContentConverter> handlers,
        ILogger<ContentHandlerSelector> logger
    ) : IPublishContentConverterSelector
{
    private readonly IPublishContentConverter[] _handlers = [.. handlers];

    public IPublishContentConverter Select(IContent content)
    {
        foreach (var handler in _handlers)
        {
            if (handler.CanConvert(content))
            {
                using var _ = logger.Scope()
                   .Add("ContentHandlerName", handler.GetType().Name)
                   .Begin();
                logger.LogDebug("已经选内容处理器");

                return handler;
            }
        }

        throw new NotSupportedException($"无法提取该输入: {content.GetType().Name}");
    }
}
