namespace MediaRelay.Metadata;


public record class DefaultMetadata : IMetadata
{
    public static DefaultMetadata Empty { get; } = new DefaultMetadata();


    public string? Title { get; init; }
    public string? Description { get; init; }

    public string? AuthorName { get; init; }
    public DateTimeOffset? PublishedAt { get; init; }

    public IReadOnlySet<string> Tags { get; init; } = new HashSet<string>();
}