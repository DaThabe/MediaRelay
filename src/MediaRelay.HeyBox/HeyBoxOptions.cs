using MediaRelay.Http;

namespace MediaRelay.HeyBox;


public sealed record class HeyBoxOptions
{
    public const string SectionName = "HeyBox";
    public const string SectionPath = $"{MediaRelayOptions.SectionPath}:{SectionName}";


    public HeyBoxHttpOptions Http { get; set; } = new();
    public HeyBoxOriginalImageUrlOptions OriginalImageUrl { get; set; } = new();
    public HeyBoxBbsLinkOptions BbsLink { get; set; } = new();
}

/// <summary>
/// Http
/// </summary>
public sealed record class HeyBoxHttpOptions
{
    public const string SectionName = nameof(HeyBoxOptions.Http);
    public const string SectionPath = $"{HeyBoxOptions.SectionPath}:{SectionName}";


    public int MaxConcurrentDownloads { get; set; } = 3;
    public HttpCookieOptions[] Cookies { get; set; } = [];
}


/// <summary>
/// OriginalImageUrl
/// </summary>
public sealed class HeyBoxOriginalImageUrlOptions
{
    public const string SectionName = nameof(HeyBoxOptions.OriginalImageUrl);
    public const string SectionPath = $"{HeyBoxOptions.SectionPath}:{SectionName}";


    public string Format { get; set; } = @"https://imgheybox1.max-c.com/bbs/{0}/{1}/{2}/{3}.{4}";
    public string Pattern { get; set; } = @"imgheybox1\.max-c\.com/bbs/(?<yyyy>\d{4})/(?<MM>\d{2})/(?<dd>\d{2})/(?<hash>[a-zA-Z0-9]+)\.(?<ext>\w+)";
    
    public string YearKey { get; set; } = "yyyy";
    public string DayKey { get; set; } = "dd";
    public string MonthKey { get; set; } = "MM";
    public string HashKey { get; set; } = "hash";
    public string ExtensionsKey { get; set; } = "ext";
}

/// <summary>
/// Artwork
/// </summary>
public sealed record class HeyBoxBbsLinkOptions
{
    public const string SectionName = nameof(HeyBoxOptions.BbsLink);
    public const string SectionPath = $"{HeyBoxOptions.SectionPath}:{SectionName}";


    public HeyBoxBbsLinkUrlOptions Url { get; set; } = new();
    public string ExtractScriptPath { get; set; } = "Browser/Scripts/HeyBoxBbsLink.js";
}
public sealed record class HeyBoxBbsLinkUrlOptions
{
    public const string SectionName = nameof(HeyBoxBbsLinkOptions.Url);
    public const string SectionPath = $"{HeyBoxBbsLinkOptions.SectionPath}:{SectionName}";


    public string Format { get; set; } = "https://www.xiaoheihe.cn/app/bbs/link/{0}";
    public string Pattern { get; set; } = @"xiaoheihe\.cn/app/bbs/link/(?<id>\d+)";
    public string LinkdKey { get; set; } = "id";
}