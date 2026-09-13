namespace MediaRelay.Source;

public record class DefaultSource : ISource
{
    public required SourceId Id { get; init; }
}