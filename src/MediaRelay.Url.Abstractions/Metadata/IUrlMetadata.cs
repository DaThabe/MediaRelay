namespace MediaRelay.Metadata;


public interface IUrlMetadata : IMetadata
{
    Uri? AuthorUrl { get; }
}
