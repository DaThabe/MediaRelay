namespace MediaRelay;


public sealed record class UrlOptions
{
    public const string SectionName = "Url";
    public const string SectionPath = $"{MediaRelayOptions.SectionPath}:{SectionName}";


    public UrlMessageQueueOptions MessageQueue { get; set; } = new();
}

public sealed record class UrlMessageQueueOptions
{
    public const string SectionName = nameof(UrlOptions.MessageQueue);
    public const string SectionPath = $"{UrlOptions.SectionPath}:{SectionName}";


    public string File { get; set; } = "UrlMessages.json";
    public int MessageRetryCount { get; set; } = 5;
}