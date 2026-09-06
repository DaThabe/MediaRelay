using MediaRelay.Http;

namespace MediaRelay.Pixiv;


public sealed record class PixivOptions
{
    public static string Name { get; set; } = "Pixiv";

    public PixivHttpOptions Http { get; set; } = new();
    public PixivOriginalImageUrlOptions OriginalImageUrl { get; set; } = new();
    public PixivArtworkOptions Artwork { get; set; } = new();
}

/// <summary>
/// Http
/// </summary>
public sealed record class PixivHttpOptions
{
    public static string Name { get; set; } = "Http";


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
    public static string Name { get; set; } = "OriginalImageUrl";


    public string Format { get; set; } = @"https://i.pximg.net/img-original/img/{0}/{1}/{2}/{3}/{4}/{5}/{6}_p{7}_{8}.{9}";
    public string Pattern { get; set; } = @"i\.pximg\.net/img-original/img/(?<yyyy>\d{4})/(?<MM>\d{2})/(?<dd>\d{2})/(?<HH>\d{2})/(?<mm>\d{2})/(?<ss>\d{2})/(?<pid>\d+)_p(?<index>\d+)\.(?<ext>\w+)";
    public string ArtworkIdKey { get; set; } = "pid";
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
    public static string Name { get; set; } = "Artwork";


    public PixivArtworkUrlOptions Url { get; set; } = new();
    public string ExtractScriptPath { get; set; } = "Browser/Scripts/PixivArtwork.js";
}
public sealed record class PixivArtworkUrlOptions
{
    public string Format { get; set; } = "https://www.pixiv.net/artworks/{0}";
    public string Pattern { get; set; } = @"pixiv\.net/artworks/(?<pid>\d+)";
    public string ArtworkIdKey { get; set; } = "pid";
}
