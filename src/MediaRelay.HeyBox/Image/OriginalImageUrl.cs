using MediaRelay.Storage;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace MediaRelay.HeyBox.Image;


internal sealed partial class OriginalImageUrl
{
    private readonly string _url;

    public required string Hash { get; init; }
    public required MediaType MediaType { get; init; }
    public required DateOnly Date { get; init; }


    private OriginalImageUrl(string url) => _url = url;
    public override string ToString() => _url;
}


internal sealed partial class OriginalImageUrl
{
    internal sealed class Parser(IOptions<HeyBoxOriginalImageUrlOptions> options)
    {
        private readonly Regex _regex = new
        (
            options.Value.Pattern,
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant
        );

        /// <summary>
        /// https://imgheybox1.max-c.com/bbs/2026/09/04/6218de37bfc858945c611101860b5dd1.jpeg
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public OriginalImageUrl Parse(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out _))
                throw new FormatException($"网址格式错误: {url}");

            var result = _regex.Match(url);
            if (!result.Success) throw new NotSupportedException($"不支持的原图网址:{url}");

            var urlRegexOptions = options.Value;

            var yyyy = int.Parse(result.Groups[urlRegexOptions.YearKey].Value);
            var MM = int.Parse(result.Groups[urlRegexOptions.MonthKey].Value);
            var dd = int.Parse(result.Groups[urlRegexOptions.DayKey].Value);

            var hash = result.Groups[urlRegexOptions.HashKey].Value.Trim();
            var ext = result.Groups[urlRegexOptions.ExtensionsKey].Value;

            var compineUrl = string.Format(options.Value.Format, yyyy, MM, dd, hash, ext);

            return new OriginalImageUrl(compineUrl)
            {
                Hash = hash,
                MediaType = MediaType.FromExtensions(ext),
                Date = new DateOnly(yyyy, MM, dd)
            };
        }
    }
}