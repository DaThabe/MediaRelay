using Microsoft.Playwright;

namespace MediaRelay.Pixiv;


public sealed record class PixivOptions
{
    public string BaseUrl { get; set; } = "https://www.pixiv.net/";
    public string Referrer { get; set; } = "https://www.pixiv.net/";

    public string UserAgent { get; set; } = "MediaRelay/1.0";
    public int MaxConcurrentDownloads { get; set; } = 3;
    public Cookie[] Cookies { get; set; } = [];
    public string ExtractScriptPath { get; set; } = "Scripts/PixivExtractScript.js";
}
