using MediaRelay.Source.Url;

namespace MediaRelay;


internal sealed class UrlRelayService(
        IUrlSourceParserSelector urlSourceParserSelector,
        ISourceRelayService sourceRelayService
    ) : IUrlRelayService
{
    public async ValueTask RelayAsync(Uri url, CancellationToken cancellationToken = default)
    {
        if (url.Scheme != Uri.UriSchemeHttp && url.Scheme != Uri.UriSchemeHttps)
            throw new ArgumentException($"不是有效的网址: {url}");

        var source = urlSourceParserSelector
                       .Select(url)
                       .Parse(url);

        await sourceRelayService.RelayAsync(source, cancellationToken);
    }
}