using MediaRelay.Image;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace MediaRelay.Pixiv.Image;


internal sealed partial class OriginalImageUrl
{
    private readonly string _url;


    public required int ArtworkId { get; init; }
    public required string Hash { get; init; }
    public required ImageFormat Format { get; init; }
    public required int Index { get; init; }
    public required DateTime UplaodAt { get; init; }


    private OriginalImageUrl(string url) => _url = url;
    public override string ToString() => _url;
}


internal sealed partial class OriginalImageUrl
{
    internal sealed class Parser(IOptions<PixivOriginalImageUrlOptions> options)
    {
        private readonly Regex _regex = new
        (
            options.Value.Pattern,
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant
        );

        /// <summary>
        /// https://i.pximg.net/img-original/img/2026/09/07/02/12/24/123456789_0.png
        /// https://i.pximg.net/img-original/img/2025/03/24/16/55/53/123456789-485cfbf98914fba1ee83201341abaf19_p0.png
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

            var pid = int.Parse(result.Groups[urlRegexOptions.ArtworkIdKey].Value);
            var hash = result.Groups[urlRegexOptions.HashKey].Value.Trim();
            var index = int.Parse(result.Groups[urlRegexOptions.IndexKey].Value);
            var ext = result.Groups[urlRegexOptions.ExtensionsKey].Value;

            var yyyy = int.Parse(result.Groups[urlRegexOptions.YearKey].Value);
            var MM = int.Parse(result.Groups[urlRegexOptions.MonthKey].Value);
            var dd = int.Parse(result.Groups[urlRegexOptions.DayKey].Value);
            var HH = int.Parse(result.Groups[urlRegexOptions.HourKey].Value);
            var mm = int.Parse(result.Groups[urlRegexOptions.MinuteKey].Value);
            var ss = int.Parse(result.Groups[urlRegexOptions.SecondKey].Value);

            var urlHash = string.IsNullOrWhiteSpace(hash) ? string.Empty : $"-{hash}";
            var compineUrl = string.Format(options.Value.Format, yyyy, MM, dd, HH, mm, ss, pid, urlHash, index, ext);

            return new OriginalImageUrl(compineUrl)
            {
                ArtworkId = pid,
                Hash = hash,
                Format = ImageFormat.FromName(ext),
                Index = index,
                UplaodAt = new DateTime(yyyy, MM, dd, HH, mm, ss)
            };
        }
    }
}