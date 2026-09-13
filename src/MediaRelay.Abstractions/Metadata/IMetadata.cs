namespace MediaRelay.Metadata;


public interface IMetadata
{
    string? Title { get; }
    string? Description { get; }
    string? AuthorName { get; }


    DateTimeOffset? PublishedAt { get; }
    IReadOnlySet<string> Tags { get; }
}
