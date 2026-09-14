using MediaRelay.Messaging.Queue;

namespace MediaRelay.Messaging.Envelope;


public interface IUrlMessageEnvelopePersistence : IMessageEnvelopePersistence<UrlMessageEnvelope, UrlMessage, Uri>;
