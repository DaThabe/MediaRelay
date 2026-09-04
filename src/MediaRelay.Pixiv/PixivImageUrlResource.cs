using MediaRelay.Resources;
using System.Text.RegularExpressions;

namespace MediaRelay.Pixiv;


internal sealed class PixivImageUrlResource(IPixivDownloader downloader) : IResource
{
    public ResourceId Id => ResourceId.Create($"pixiv:{Pid}_p{Index}");
    public required string Url { get; init; }
    public required long Pid { get; init; }
    public required long Index { get; init; }
    public required string Extensions { get; init; }


    public ValueTask<Stream> GetStreamAsync(CancellationToken cancellationToken = default)
    {
        return downloader.DownloadAsync(Url, cancellationToken);
    }

    public static PixivImageUrlResource CreateFromUrl(string url, IPixivDownloader downloader)
    {
        // https://i.pximg.net/img-original/img/2026/07/11/04/26/56/147058530_p0.png
        var fileName = Path.GetFileName(url);
        var match = Regex.Match(fileName, @"(?<pid>\d+)_p(?<index>\d+).*\.(?<ext>.+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1));
        if (!match.Success) throw new ArgumentException($"Invalid Pixiv URL: {url}");


        long pid = long.Parse(match.Groups["pid"].Value);
        long index = long.Parse(match.Groups["index"].Value);
        string ext = match.Groups["ext"].Value;

        return new PixivImageUrlResource(downloader) { Url = url, Pid = pid, Index = index, Extensions = ext };
    }
}