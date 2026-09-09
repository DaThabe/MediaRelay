namespace MediaRelay.Browser;


public sealed record class BrowserLaunchOptions
{
    public const string SectionName = nameof(BrowserOptions.Launch);
    public const string SectionPath = $"{BrowserOptions.SectionPath}:{SectionName}";



    public bool Headless { get; set; }
    public string? ExecutablePath { get; set; }
    public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(1);
    public int MaxConcurrentPages { get; set; } = 10;
    public string[] Args { get; set; } = [];
    public string? UserAgent { get; set; }
    public string? DownloadsPath { get; set; } = $"./Browser/downloads";
}