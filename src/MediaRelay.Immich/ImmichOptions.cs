namespace MediaRelay.Immich;


public sealed record class ImmichOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
}
