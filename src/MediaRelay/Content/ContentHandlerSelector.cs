using MediaRelay.Content;
using MediaRelay.Publish;
using Microsoft.Extensions.Logging;

namespace MediaRelay;

internal sealed partial class ContentHandlerSelector(
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
                LogSelected(handler.GetType().Name, content);
                return handler;
            }
        }

        throw new NotSupportedException($"无法提取该输入: {content.GetType().Name}");
    }


    [LoggerMessage(Level = LogLevel.Debug, Message = "已选择 [{HandlerType}] < 内容 [{Content}]")]
    private partial void LogSelected(string handlerType, IContent content);
}
