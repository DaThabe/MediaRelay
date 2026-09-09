namespace MediaRelay.Clipboard;


public sealed record class ClipboardOptions
{
    public const string SectionName = "Clipboard";
    public const string SectionPath = $"{MediaRelayOptions.SectionPath}:{SectionName}";


    public TimeSpan PollingInterval { get; set; } = TimeSpan.FromMilliseconds(500);
}