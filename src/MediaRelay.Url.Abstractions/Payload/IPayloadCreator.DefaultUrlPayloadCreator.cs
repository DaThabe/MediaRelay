using MediaRelay.Content;
using MediaRelay.Storage;

namespace MediaRelay.Payload;

public sealed class DefaultUrlPayloadCreator(
    IResourceStorage resourceStorage) : UrlContentPayloadCreator<DefaultUrlContent>(resourceStorage);