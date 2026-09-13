namespace MediaRelay.Metadata;


public interface IUrlMetadata : IMetadata
{
    Uri? AuthorUrl { get; }
}


public record class DefaultUrlMetadata : DefaultMetadata, IUrlMetadata
{
    public static new DefaultUrlMetadata Empty { get; } = new DefaultUrlMetadata();


    public Uri? AuthorUrl { get; init; }
}