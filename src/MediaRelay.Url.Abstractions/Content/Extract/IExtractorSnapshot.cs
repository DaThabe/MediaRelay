namespace MediaRelay.Content.Extract;


public interface IExtractorSnapshot
{
    string[] Resources { get; }
    string? Title { get; }
    string? Content { get; }
    DateTimeOffset? UploadAt { get; }
    string? AuthorName { get; }
    string? AuthorUrl { get; }
    string[] Tags { get; }
}