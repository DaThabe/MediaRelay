namespace MediaRelay.Metadata;


public interface IMetadataBuilder<TBuilder, TMetadata>
    where TBuilder : IMetadataBuilder<TBuilder, TMetadata>
    where TMetadata : IMetadata
{
    TBuilder SetTitle(string? title);
    TBuilder SetDescription(string? description);

    TBuilder SetAuthorName(string? name);
    TBuilder SetPublishedAt(DateTimeOffset? time);

    TBuilder AddTags(params IEnumerable<string> tags);

    TMetadata Build();
}