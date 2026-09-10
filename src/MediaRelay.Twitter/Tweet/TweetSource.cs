using MediaRelay.Source;
using MediaRelay.Url;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace MediaRelay.Twitter.Tweet;


internal sealed partial record class TweetSource : IUrlSource
{
    public required SourceId Id { get; init; }
    public required Uri Url { get; init; }
    public required string Username { get; init; }
    public required string TweetId { get; init; }


    private TweetSource() { }
}

// Factory
internal sealed partial record class TweetSource
{
    internal sealed class UrlParser(IOptions<TwitterTweetUrlOptions> options) : IUrlSourceParser
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
            if (!result.Success) throw new NotSupportedException($"不持支解析的推文网址 {url}");

            var uid = result.Groups[options.Value.UsernameKey].Value;
            var tid = result.Groups[options.Value.TweetIdKey].Value;

            return new TweetSource()
            {
                Id = SourceId.FromUsernameAndTweetId(uid, tid),
                Username = uid,
                TweetId = tid,
                Url = new Uri(string.Format(options.Value.Format, uid, tid))
            };
        }
    }
}