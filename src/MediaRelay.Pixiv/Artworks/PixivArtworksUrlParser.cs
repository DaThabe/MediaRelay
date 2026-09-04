using MediaRelay.Input;
using MediaRelay.Source;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace MediaRelay.Pixiv.Artworks;


internal sealed partial class PixivArtworksUrlSourceParser(IOptions<PixivOptions> options) : IInputParser
{
    public bool CanParse(string input) => UrlRegex().IsMatch(input);

    public ISource Parse(string input)
    {
        var result = UrlRegex().Match(input);

        if (!long.TryParse(result.Groups["pid"].Value, out var pid))
        {
            throw new ArgumentException();
        }

        var url = new Uri(new Uri(options.Value.BaseUrl), $"/artworks/{pid}");
        return new PixivArtworkSource(url, pid);
    }

    [GeneratedRegex(@"pixiv\.net/artworks/(?<pid>\d+)")]
    private static partial Regex UrlRegex();
}