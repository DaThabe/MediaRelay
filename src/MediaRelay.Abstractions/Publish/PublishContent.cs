namespace MediaRelay.Publish;


public sealed record class PublishContent
{
    public required ContentId ContentId { get; init; }
    public required IReadOnlySet<Uri> Resources { get; init; }


    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;


    public string? Author { get; init; }
    public string? AuthorUrl { get; init; }


    public IReadOnlySet<string> Tags { get; init; } = new HashSet<string>();
}