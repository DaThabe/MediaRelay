using MediaRelay.Content;
using MediaRelay.Storage.Resource;

namespace MediaRelay.Payload;

public sealed class DefaultUrlPayloadCreator(
    IResourceRepository resourceStorage) : UrlContentPayloadCreator<DefaultUrlContent>(resourceStorage);