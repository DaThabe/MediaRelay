using MediaRelay.Http;
using System.Text.Json.Serialization;

namespace MediaRelay.Twitter;


public sealed record class TwitterOptions
{
    public static string Name { get; set; } = "Twitter";


    [JsonPropertyName(TwitterHttpOptions.Name)]
    public TwitterHttpOptions Http { get; set; } = new();


    [JsonPropertyName(TwitterImageUrlOptions.Name)]
    public TwitterImageUrlOptions ImageUrlRegex { get; set; } = new();


    [JsonPropertyName(TwitterTweetOptions.Name)]
    public TwitterTweetOptions Tweet { get; set; } = new();
}


/// <summary>
/// Http
/// </summary>
public sealed record class TwitterHttpOptions
{
    public const string Name = "Http";


    public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(1);
    public HttpCookieOptions[] Cookies { get; set; } = [];
    public int MaxConcurrentDownloads { get; set; } = 3;
}

/// <summary>
/// Image-Regex
/// </summary>
public sealed class TwitterImageUrlOptions
{
    public const string Name  = "ImageUrl";


    public string Format { get; set; } = @"https://pbs.twimg.com/media/{0}?format={1}&name={2}";
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
    public const string Name = "Tweet";


    [JsonPropertyName(TwitterTweetUrlOptions.Name)]
    public TwitterTweetUrlOptions Url { get; set; } = new();
    
    public string ExtractScriptPath { get; set; } = "Browser/Scripts/TwitterTweet.js";
}
public sealed record class TwitterTweetUrlOptions
{
    public const string Name = "Url";

    public string Format { get; set; } = "https://x.com/{0}/status/{1}";
    public string Pattern { get; set; } = @"x\.com/(?<uid>\w+)/status/(?<tid>\w+)\?*";
    public string UsernameKey { get; set; } = "uid";
    public string TweetIdKey { get; set; } = "tid";
}
