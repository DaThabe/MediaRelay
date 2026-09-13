namespace MediaRelay.Metadata;


public record class DefaultUrlMetadata : DefaultMetadata, IUrlMetadata
{
    public static new DefaultUrlMetadata Empty { get; } = new DefaultUrlMetadata();


    public Uri? AuthorUrl { get; init; }
}