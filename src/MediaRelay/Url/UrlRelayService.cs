using MediaRelay.Source;

namespace MediaRelay.Url;


internal sealed class UrlRelayService(
        IUrlSourceFactory urlSourceFactory,
        ISourceRelayService sourceRelayService
    ) : IUrlRelayService
{
    public async ValueTask RelayAsync(Uri url, CancellationToken cancellationToken = default)
    {
        if (url.Scheme != Uri.UriSchemeHttp && url.Scheme != Uri.UriSchemeHttps)
            throw new ArgumentException($"不是有效的网址: {url}");

        var source = urlSourceFactory
            .Create(url);

        await sourceRelayService
            .RelayAsync(source, cancellationToken);
    }
}