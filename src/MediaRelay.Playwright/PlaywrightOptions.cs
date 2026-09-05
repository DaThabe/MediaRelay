namespace MediaRelay.Playwright;


public sealed record class PlaywrightOptions
{
    public static string Name { get; set; } = "Playwright";

    public required BrowserLaunchOptions BrowserLaunch { get; set; }
}


public sealed record class BrowserLaunchOptions
{
    public bool Headless { get; set; }
    public string? ExecutablePath { get; set; }
    public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(1);
    public int MaxConcurrentPages { get; set; } = 10;
    public string[] Args { get; set; } = [];
    public string? UserAgent { get; set; }
    public string? DownloadsPath { get; set; } = $"./.{PlaywrightOptions.Name}/Browser/downloads";
}