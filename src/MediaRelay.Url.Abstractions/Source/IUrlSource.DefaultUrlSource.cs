namespace MediaRelay.Source;


public record class DefaultUrlSource : IUrlSource
{
    public required Uri Url { get; init; }
    public required SourceId Id { get; init; }
}