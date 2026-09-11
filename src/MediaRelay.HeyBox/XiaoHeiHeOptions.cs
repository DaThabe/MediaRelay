using MediaRelay.Http;

namespace MediaRelay.HeyBox;


public sealed record class XiaoHeiHeOptions
{
    public const string SectionName = "XiaoHeiHe";
    public const string SectionPath = $"{MediaRelayOptions.SectionPath}:{SectionName}";


    public PixivHttpOptions Http { get; set; } = new();
    public PixivOriginalImageUrlOptions OriginalImageUrl { get; set; } = new();
    public PixivArtworkOptions Artwork { get; set; } = new();
}

/// <summary>
/// Http
/// </summary>
public sealed record class PixivHttpOptions
{
    public const string SectionName = nameof(XiaoHeiHeOptions.Http);
    public const string SectionPath = $"{XiaoHeiHeOptions.SectionPath}:{SectionName}";


    public string BaseUrl { get; set; } = "https://www.pixiv.net/";
    public string Referrer { get; set; } = "https://www.pixiv.net/";
    public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(1);

    public int MaxConcurrentDownloads { get; set; } = 3;
    public HttpCookieOptions[] Cookies { get; set; } = [];
}


/// <summary>
/// OriginalImageUrl
/// </summary>
public sealed class PixivOriginalImageUrlOptions
{
    public const string SectionName = nameof(XiaoHeiHeOptions.OriginalImageUrl);
    public const string SectionPath = $"{XiaoHeiHeOptions.SectionPath}:{SectionName}";


    public string Format { get; set; } = @"https://i.pximg.net/img-original/img/{0}/{1}/{2}/{3}/{4}/{5}/{6}_p{7}_{8}.{9}";
    public string Pattern { get; set; } = @"i\.pximg\.net/img-original/img/(?<yyyy>\d{4})/(?<MM>\d{2})/(?<dd>\d{2})/(?<HH>\d{2})/(?<mm>\d{2})/(?<ss>\d{2})/(?<pid>\d+)(-(?<hash>[a-zA-Z0-9]+)){0,1}_p(?<idx>\d+)\.(?<ext>\w+)";
    public string ArtworkIdKey { get; set; } = "pid";
    public string HashKey { get; set; } = "hash";
    public string IndexKey { get; set; } = "idx";
    public string ExtensionsKey { get; set; } = "ext";
    public string YearKey { get; set; } = "yyyy";
    public string MonthKey { get; set; } = "MM";
    public string DayKey { get; set; } = "dd";
    public string HourKey { get; set; } = "HH";
    public string MinuteKey { get; set; } = "mm";
    public string SecondKey { get; set; } = "ss";
}

/// <summary>
/// Artwork
/// </summary>
public sealed record class PixivArtworkOptions
{
    public const string SectionName = nameof(XiaoHeiHeOptions.Artwork);
    public const string SectionPath = $"{XiaoHeiHeOptions.SectionPath}:{SectionName}";


    public HeyBoxBbsLinkUrlOptions Url { get; set; } = new();
    public string ExtractScriptPath { get; set; } = "Browser/Scripts/PixivArtwork.js";
}
public sealed record class HeyBoxBbsLinkUrlOptions
{
    public const string SectionName = nameof(PixivArtworkOptions.Url);
    public const string SectionPath = $"{PixivArtworkOptions.SectionPath}:{SectionName}";


    public string Format { get; set; } = "https://www.pixiv.net/artworks/{0}";
    public string Pattern { get; set; } = @"pixiv\.net/artworks/(?<pid>\d+)";
    public string LinkdKey { get; set; } = "pid";
}