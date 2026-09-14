using MediaRelay.Messaging.Envelope;
using Microsoft.Extensions.Options;

namespace MediaRelay.Messaging.Queue.Envelope;


internal sealed class UrlMessageEnvelopeCreator(IOptions<UrlMessageQueueOptions> options) : IUrlMessageEnvelopeCreator
{
    public bool CanCreate(UrlMessage message) => true;

    public UrlMessageEnvelope Create(UrlMessage message)
    {
        return UrlMessageEnvelope.Create(message, UrlMessageRetryCounter.FromCount(options.Value.RetryCount));
    }
}