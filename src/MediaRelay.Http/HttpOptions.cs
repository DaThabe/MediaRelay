namespace MediaRelay.Http;


public sealed record class HttpOptions
{
    public bool IgnoreSslErrors { get; set; }
    public ProxyOptions? Proxy { get; set; }
    public TimeSpan Timeout { get; init; } = TimeSpan.FromMinutes(3);
    public string? UserAgent { get; init; }
}

public sealed record class ProxyOptions
{
    public bool Enable { get; init; } = false;
    public string Address { get; init; } = "http://127.0.0.1:7990";
}