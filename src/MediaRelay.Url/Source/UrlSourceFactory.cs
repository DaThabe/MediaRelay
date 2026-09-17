using Microsoft.Extensions.Logging;

namespace MediaRelay.Source;


internal sealed class UrlSourceFactory(
        IEnumerable<IUrlSourceParser> parsers
    ) : IUrlSourceFactory
{
    private readonly IUrlSourceParser[] _parsers = [.. parsers];

    public IUrlSource Create(Uri url)
    {
        return Parse(url) ??
            throw new NotSupportedException($"无法解析的网址: {url}");
    }

    private IUrlSource? Parse(Uri url)
    {
        foreach (var parser in _parsers)
        {
            if (!parser.CanParse(url)) continue;
            return parser.Parse(url);
        }

        return null;
    }
}