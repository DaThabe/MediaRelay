using MediaRelay.Image;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace MediaRelay.Twitter.Image;

internal sealed partial class ImageUrl
{
    private readonly string _url;

    public required string MediaId { get; init; }
    public required ImageFormat Format { get; init; }
    public ImageSize Size { get; init; } = ImageSize.Original;


    private ImageUrl(string url) => _url = url;
    public override string ToString() => _url;
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
        public ImageUrl Parse(string url, ImageSize? size = null)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var _))
                throw new FormatException($"不是有效的网址: {url}");

            var result = _regex.Match(url);
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

            return new ImageUrl(originalUrl)
            {
                MediaId = mediaId,
                Format = ImageFormat.FromName(format),
                Size = useSize
            };
        }
    }
}