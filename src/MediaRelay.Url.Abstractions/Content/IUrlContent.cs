using MediaRelay.Resources;
using MediaRelay.Source;

namespace MediaRelay.Content;


public interface IUrlContent : IContent
{
    ISource IContent.Source => Source;
    new IUrlSource Source { get; }


    string? Title { get; }
    string? Content { get; }


    string? AuthorName { get; }
    Uri? AuthorUrl { get; }


    DateTimeOffset? UploadAt { get; }
    IReadOnlySet<string> Tags { get; }
}


public abstract record class UrlContent : IUrlContent
{
    public required ContentId Id { get; init; }
    public required IUrlSource Source { get; init; }
    public required IReadOnlySet<IResource> Resources { get; init; }


    public string? Title { get; init; }
    public string? Content { get; init; }

    public string? AuthorName { get; init; }
    public Uri? AuthorUrl { get; init; }


    public DateTimeOffset? UploadAt { get; init; }
    public IReadOnlySet<string> Tags { get; init; } = new HashSet<string>();
}