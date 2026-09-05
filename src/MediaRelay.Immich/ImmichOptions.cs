namespace MediaRelay.Immich;


public sealed record class ImmichOptions
{
    public static string Name { get; set; } = "Immich";


    public required string BaseUrl { get; set; }
    public required string ApiKey { get; set; }
}
