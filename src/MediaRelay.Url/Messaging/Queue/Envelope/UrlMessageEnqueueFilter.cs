using MediaRelay.Source;

namespace MediaRelay.Messaging.Queue.Envelope;


internal sealed class UrlMessageEnqueueFilter(IEnumerable<IUrlSourceParser> urlSourceParsers) : IUrlMessageEnqueueFilter
{
    public readonly IUrlSourceParser[] _urlSourceParsers = [.. urlSourceParsers];

    public bool CanEnqueue(UrlMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);

        foreach (var i in _urlSourceParsers)
        {
            if (i.CanParse(message.Content)) return true;
        }

        return false;
    }
}
