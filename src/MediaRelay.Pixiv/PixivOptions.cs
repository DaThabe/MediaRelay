using MediaRelay.Http;

namespace MediaRelay.Pixiv;


public sealed record class PixivOptions
{
    public const string SectionName = "Pixiv";
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
    public const string SectionName = nameof(PixivOptions.Http);
    public const string SectionPath = $"{PixivOptions.SectionPath}:{SectionName}";


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
    public const string SectionName = nameof(PixivOptions.OriginalImageUrl);
    public const string SectionPath = $"{PixivOptions.SectionPath}:{SectionName}";


    public string Format { get; set; } = @"https://i.pximg.net/img-original/img/{0:D4}/{1:D2}/{2:D2}/{3:D2}/{4:D2}/{5:D2}/{6}_p{7}_{8}.{9}";
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
    public const string SectionName = nameof(PixivOptions.Artwork);
    public const string SectionPath = $"{PixivOptions.SectionPath}:{SectionName}";


    public PixivArtworkUrlOptions Url { get; set; } = new();
    public string ExtractScriptPath { get; set; } = "Browser/Scripts/PixivArtwork.js";
}
public sealed record class PixivArtworkUrlOptions
{
    public const string SectionName = nameof(PixivArtworkOptions.Url);
    public const string SectionPath = $"{PixivArtworkOptions.SectionPath}:{SectionName}";


    public string Format { get; set; } = "https://www.pixiv.net/artworks/{0}";
    public string Pattern { get; set; } = @"pixiv\.net/artworks/(?<pid>\d+)";
    public string ArtworkIdKey { get; set; } = "pid";
}