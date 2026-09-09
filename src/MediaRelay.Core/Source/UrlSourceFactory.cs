using Microsoft.Extensions.Logging;

namespace MediaRelay.Source;


internal sealed class UrlSourceFactory(
        IEnumerable<IUrlSourceParser> parsers,
        ILogger<UrlSourceFactory> logger
    ) : IUrlSourceFactory
{
    private readonly IUrlSourceParser[] _parsers = [.. parsers];

    public IUrlSource Create(Uri url)
    {
        foreach (var parser in _parsers)
        {
            if (!parser.CanParse(url)) continue;

            using var _ = logger.Scope()
                .Add("ParserName", parser.GetType().Name)
                .Begin();
            logger.LogDebug("已经选择网址来源解析器");

            return parser.Parse(url);
        }

        throw new NotSupportedException($"无法解析的网址: {url}");
    }
}