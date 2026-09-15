using MediaRelay.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace MediaRelay.Twitter.Image;

internal sealed partial class ImageUrl
{
    public Uri Uri { get; }
    public required string MediaId { get; init; }
    public required MediaType MediaType { get; init; }
    public ImageSize Size { get; init; } = ImageSize.Original;


    private ImageUrl(Uri url) => Uri = url;
    public override string ToString() => Uri.ToString();
}


internal sealed partial class ImageUrl
{
    internal sealed class Parser(IOptions<TwitterImageUrlOptions> options, ILogger<Parser> logger)
    {
        private readonly Regex _regex = new
        (
            options.Value.Pattern,
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant
        );

        /// <summary>
        /// 这种格式的网址 https://pbs.twimg.com/media/ABCD123456789?format=jpg&name=4096x4096
        /// </summary>
        /// <exception cref="FormatException"></exception>
        public ImageUrl Parse(Uri url, ImageSize? size = null)
        {
            var result = _regex.Match(url.ToString());
            if (!result.Success) throw new NotSupportedException($"不支持的推特图像网址: {url}");

            var urlRegexOptions = options.Value;

            var mediaId = result.Groups[urlRegexOptions.MediaIdKey].Value;
            var format = result.Groups[urlRegexOptions.FormatKey].Value;
            var sizeName = result.Groups[urlRegexOptions.SizeKey].Value;
            var useSize = size ?? ImageSize.FromName(sizeName);
            var originalUrl = string.Format(options.Value.Format, mediaId, format, useSize);

            using var _ = logger.Scope("MediaId", mediaId)
                .Add("Format", format)
                .Add("Size", useSize)
                .Add("Url", originalUrl)
                .Begin();
            logger.LogInformation("已解析到推文媒体网址");

            return new ImageUrl(new Uri(originalUrl))
            {
                MediaId = mediaId,
                MediaType = MediaType.FromExtensions(format),
                Size = useSize
            };
        }
    }
}