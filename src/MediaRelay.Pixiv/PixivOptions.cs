using Microsoft.Playwright;

namespace MediaRelay.Pixiv;


public sealed record class PixivOptions
{
    public static string Name { get; set; } = "Immich";


    public string BaseUrl { get; set; } = "https://www.pixiv.net/";
    public string Referrer { get; set; } = "https://www.pixiv.net/";
    public int MaxConcurrentDownloads { get; set; } = 3;
    public required Cookie[] Cookies { get; set; }
    public string ExtractScriptPath { get; set; } = "Scripts/PixivExtractScript.js";
}