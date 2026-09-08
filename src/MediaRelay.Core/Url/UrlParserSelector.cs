using Microsoft.Extensions.Logging;

namespace MediaRelay.Url;


internal sealed class UrlParserSelector(
        IEnumerable<IUrlParser> parsers,
        ILogger<UrlParserSelector> logger
    ) : IUrlParserSelector
{
    private readonly IUrlParser[] _parsers = [.. parsers];

    public IUrlParser Select(Uri url)
    {
        foreach (var parser in _parsers)
        {
            if (!parser.CanParse(url)) continue;

            using var _ = logger.Scope()
                .Add("ParserName", parser.GetType().Name)
                .Begin();
            logger.LogDebug("已经选择网址来源解析器");

            return parser;
        }

        throw new NotSupportedException($"无法解析的网址: {url}");
    }
}