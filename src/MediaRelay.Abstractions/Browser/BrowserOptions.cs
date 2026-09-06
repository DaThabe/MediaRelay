namespace MediaRelay.Browser;


public sealed record class BrowserOptions
{
    public static string Name { get; set; } = "Browser";

    public BrowserLaunchOptions Launch { get; set; } = new();
}