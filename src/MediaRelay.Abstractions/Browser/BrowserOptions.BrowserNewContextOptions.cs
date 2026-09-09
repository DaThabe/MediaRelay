using System.Text.Json.Serialization;

namespace MediaRelay.Browser;


public sealed record class BrowserNewContextOptions
{
    public const string SectionName = nameof(BrowserOptions.NewContext);
    public const string SectionPath = $"{BrowserOptions.SectionPath}:{SectionName}";


    public ViewportSize? ViewportSize { get; set; }
}


public record struct ViewportSize(int Width, int Height);


public sealed record class PageGotoOptions
{
    public WaitUntilState? WaitUntil { get; set; }
    public TimeSpan? Timeout { get; set; }
    public string? Referer { get; set; }
}


[JsonConverter(typeof(JsonStringEnumConverter<WaitUntilState>))]
public enum WaitUntilState
{
    Load,
    DOMContentLoaded,
    NetworkIdle,
    Commit
}