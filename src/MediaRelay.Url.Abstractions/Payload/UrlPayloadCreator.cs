using MediaRelay.Content;
using MediaRelay.Storage;

namespace MediaRelay.Payload;


public class UrlPayloadCreator(
    IResourceStorage resourceStorage) : UrlPayloadCreator<DefaultUrlContent>(resourceStorage);