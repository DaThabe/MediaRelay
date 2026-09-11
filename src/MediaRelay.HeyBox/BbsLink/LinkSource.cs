using MediaRelay.Source;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace MediaRelay.HeyBox.BbsLink;


internal sealed partial record class LinkSource : IUrlSource
{
    public required SourceId Id { get; init; }
    public required Uri Url { get; init; }
    public required long LinkId { get; init; }

    private LinkSource() { }
}

// Factory
internal sealed partial record class LinkSource
{
    internal sealed class UrlParser(IOptions<HeyBoxBbsLinkUrlOptions> options) : IUrlSourceParser
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

            var id = long.Parse(result.Groups[options.Value.LinkdKey].Value);

            return new LinkSource()
            {
                Id = SourceId.FromHeyBoxBbsLinkId(id),
                Url = new Uri(string.Format(options.Value.Format, id)),
                LinkId = id
            };
        }
    }
}