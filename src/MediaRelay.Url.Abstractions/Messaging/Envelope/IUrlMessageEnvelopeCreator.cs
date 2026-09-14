using MediaRelay.Messaging.Queue;

namespace MediaRelay.Messaging.Envelope;


public interface IUrlMessageEnvelopeCreator : IMessageEnvelopeCreator<UrlMessageEnvelope, UrlMessage, Uri>;