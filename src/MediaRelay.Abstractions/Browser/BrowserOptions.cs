namespace MediaRelay.Browser;


public sealed record class BrowserOptions
{
    public const string SectionName = nameof(MediaRelayOptions.Browser);
    public const string SectionPath = $"{MediaRelayOptions.SectionPath}:{SectionName}";


    public BrowserLaunchOptions Launch { get; set; } = new();
    public BrowserNewContextOptions NewContext { get; set; } = new();
}