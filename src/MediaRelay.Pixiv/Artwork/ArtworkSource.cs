using MediaRelay.Source;
using MediaRelay.Url;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace MediaRelay.Pixiv.Artwork;


internal sealed partial record class ArtworkSource : IUrlSource
{
    public required SourceId Id { get; init; }
    public required Uri Url { get; init; }
    public required long ArtworkId { get; init; }

    private ArtworkSource() { }
}

// Factory
internal sealed partial record class ArtworkSource
{
    internal sealed class UrlParser(IOptions<PixivArtworkUrlOptions> options) : IUrlParser
    {
        private readonly Regex _regex = new
        (
            options.Value.Pattern,
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant
        );

        public bool CanParse(Uri url) => _regex.IsMatch(url.ToString());

        public IUrlSource Parse(Uri url)
        {
            var result = _regex.Match(url.ToString());

            var pid = long.Parse(result.Groups[options.Value.ArtworkIdKey].Value);

            return new ArtworkSource()
            {
                Id = SourceId.FromPixivArtworkId(pid),
                Url = new Uri(string.Format(options.Value.Format, pid)),
                ArtworkId = pid
            };
        }
    }
}