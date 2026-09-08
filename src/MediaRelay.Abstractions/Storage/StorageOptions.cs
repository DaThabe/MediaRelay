namespace MediaRelay.Storage;


public sealed record StorageOptions
{
    public const string SectionName = nameof(MediaRelayOptions.Storage);
    public const string SectionPath = $"{MediaRelayOptions.SectionPath}:{SectionName}";


    public string RootPath { get; set; } = "./Storage";
}