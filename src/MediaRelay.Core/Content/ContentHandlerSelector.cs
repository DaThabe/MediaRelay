using Microsoft.Extensions.Logging;

namespace MediaRelay.Content;


internal sealed class ContentHandlerSelector(
        IEnumerable<IContentConverter> converters,
        ILogger<ContentHandlerSelector> logger
    ) : IContentConverterSelector
{
    private readonly IContentConverter[] _converters = [.. converters];

    public IContentConverter Select(IContent content)
    {
        foreach (var converter in _converters)
        {
            if (converter.CanConvert(content))
            {
                using var _ = logger.Scope()
                   .Add("ContentHandlerName", converter.GetType().Name)
                   .Begin();
                logger.LogDebug("已经选内容处理器");

                return converter;
            }
        }

        throw new NotSupportedException($"无法提取该输入: {content.GetType().Name}");
    }
}
