using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MediaRelay.Http;

internal sealed class HttpClient : IHttpClient
{
    private readonly System.Net.Http.HttpClient _inner;
    private readonly ILogger<HttpClient> _logger;


    public HttpClient(IOptions<HttpOptions> options, ILogger<HttpClient> logger)
    {
        var handler = new HttpClientHandler();

        if (options.Value.IgnoreSslErrors)
        {
            handler.ServerCertificateCustomValidationCallback = delegate { return true; };
        }

        _inner = new System.Net.Http.HttpClient(handler)
        {
            Timeout = options.Value.Timeout
        };

        if (!string.IsNullOrEmpty(options.Value.UserAgent))
        {
            _inner.DefaultRequestHeaders.UserAgent.ParseAdd(options.Value.UserAgent);
        }

        _logger = logger;
    }

    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        LogReuqest(request);
        return _inner.SendAsync(request, cancellationToken);
    }


    private void LogReuqest(HttpRequestMessage requestMessage)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug("{Method} Url={Url}", requestMessage.Method, requestMessage.RequestUri);
    }
}