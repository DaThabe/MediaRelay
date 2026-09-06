namespace MediaRelay.Http;


public sealed record class HttpOptions
{
    public static string Name { get; set; } = "Http";


    public bool IgnoreSslErrors { get; set; }
    public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(3);
    public string? UserAgent { get; set; }
    public HttpCookieOptions[] Cookies { get; set; } = [];
}


public sealed record class HttpCookieOptions
{
    public string Name { get; set; } = default!;
    public string Value { get; set; } = default!;
    public string? Domain { get; set; }
    public string? Path { get; set; }

    public DateTimeOffset? Expires { get; set; }
    public bool? HttpOnly { get; set; }
    public bool? Secure { get; set; }

    public string? Url { get; set; }
    public string? PartitionKey { get; set; }
    public HttpCookieSameSite? SameSite { get; set; }
}


public enum HttpCookieSameSite
{
    Strict,
    Lax,
    None,
}