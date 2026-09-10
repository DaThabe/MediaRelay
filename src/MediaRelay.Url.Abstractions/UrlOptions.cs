namespace MediaRelay;


public sealed record class UrlOptions
{
    public const string SectionName = "Url";
    public const string SectionPath = $"{MediaRelayOptions.SectionPath}:{SectionName}";


    public string QueueFile { get; set; } = "UrlMessages.json";
}