namespace MediaRelay.Http;


public sealed record class HttpOptions
{
    public static string Name { get; set; } = "Http";


    public bool IgnoreSslErrors { get; set; }
    public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(3);
    public string? UserAgent { get; set; }
}