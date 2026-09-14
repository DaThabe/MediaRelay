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


    public string FilePath { get; set; } = "UrlMessages.json";

    public int RetryCount { get; set; } = 5;
    public TimeSpan ErrorRetryDelay { get; set; } = TimeSpan.FromSeconds(1);

    public TimeSpan ProcessingTimeout { get; set; } = TimeSpan.FromMinutes(2);
}