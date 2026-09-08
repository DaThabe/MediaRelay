using MediaRelay.Http;
using System.Text.Json.Serialization;

namespace MediaRelay.Twitter;


public sealed record class TwitterOptions
{
    public const string SectionName = "Twitter";
    public const string SectionPath = $"{MediaRelayOptions.SectionPath}:{SectionName}";


    public TwitterHttpOptions Http { get; set; } = new();
    public TwitterImageUrlOptions ImageUrl { get; set; } = new();
    public TwitterTweetOptions Tweet { get; set; } = new();
}


/// <summary>
/// Http
/// </summary>
public sealed record class TwitterHttpOptions
{
    public const string SectionName = nameof(TwitterOptions.Http);
    public const string SectionPath = $"{TwitterOptions.SectionName}:{SectionName}";


    public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(1);
    public HttpCookieOptions[] Cookies { get; set; } = [];
    public int MaxConcurrentDownloads { get; set; } = 3;
}

/// <summary>
/// Image-Regex
/// </summary>
public sealed class TwitterImageUrlOptions
{
    public const string SectionName = nameof(TwitterOptions.ImageUrl);
    public const string SectionPath = $"{TwitterOptions.SectionPath}:{SectionName}";


    public string Format { get; set; } = "https://pbs.twimg.com/media/{0}?format={1}&name={2}";
    public string Pattern { get; set; } = @"pbs\.twimg\.com/media/(?<mid>[A-Za-z0-9\-_]+)\?*(?:format=(?<fmt>[^&]+)&?|name=(?<size>[^&]+)&?)*";
    public string MediaIdKey { get; set; } = "mid";
    public string FormatKey { get; set; } = "fmt";
    public string SizeKey { get; set; } = "size";
}

/// <summary>
/// Tweet
/// </summary>
public sealed record class TwitterTweetOptions
{
    public const string SectionName = nameof(TwitterOptions.Tweet);
    public const string SectionPath = $"{TwitterOptions.SectionPath}:{SectionName}";


    public TwitterTweetUrlOptions Url { get; set; } = new();
    public string ExtractScriptPath { get; set; } = "Browser/Scripts/TwitterTweet.js";
    public string VideoDownloadUrl { get; set; } = "https://savetwitter.net/";
    public string VideoDownloadUrlScriptPath { get; set; } = "Browser/Scripts/TwitterTweetVideoDownloadUrl.js";
}
public sealed record class TwitterTweetUrlOptions
{
    public const string SectionName = nameof(TwitterTweetOptions.Url);
    public const string SectionPath = $"{TwitterTweetOptions.SectionPath}:{SectionName}";


    public string Format { get; set; } = "https://x.com/{0}/status/{1}";
    public string Pattern { get; set; } = @"x\.com/(?<uid>\w+)/status/(?<tid>\w+)\?*";
    public string UsernameKey { get; set; } = "uid";
    public string TweetIdKey { get; set; } = "tid";
}
