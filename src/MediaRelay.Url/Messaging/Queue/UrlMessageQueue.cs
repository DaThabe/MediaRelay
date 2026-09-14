using MediaRelay.Messaging.Envelope;
using Microsoft.Extensions.Logging;

namespace MediaRelay.Messaging.Queue;


internal sealed class UrlMessageQueue(
        IUrlMessageEnvelopePersistence envelopePersistence,
        IUrlMessageEnvelopeCreator envelopeCreator,
        IUrlMessageEnqueueFilter enqueueFilter,
        ILogger<UrlMessageQueue> logger
    ) : PersistenceMessageQueue<UrlMessageEnvelope, UrlMessage, Uri>(
            envelopePersistence: envelopePersistence,
            envelopeCreator: envelopeCreator,
            enqueueFilter: enqueueFilter,
            logger: logger);