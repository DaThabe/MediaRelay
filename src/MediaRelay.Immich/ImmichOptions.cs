namespace MediaRelay.Immich;


public sealed record class ImmichOptions
{
    public const string SectionName = "Immich";
    public const string SectionPath = $"{MediaRelayOptions.SectionPath}:{SectionName}";


    public required string BaseUrl { get; set; }
    public required string ApiKey { get; set; }
}