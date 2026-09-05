using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;

namespace MediaRelay.Http;


public interface IHttpClient
{
    Task<HttpResponseMessage> GetAsync(string url, CancellationToken cancellationToken = default);
    Task<string> GetStringAsync(string url, CancellationToken cancellationToken = default);
    Task<byte[]> GetByteArrayAsync(string url, CancellationToken cancellationToken = default);
    Task<Stream> GetStreamAsync(string url, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
}


internal sealed partial class HttpClient : IHttpClient
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

    public Task<HttpResponseMessage> GetAsync(string url, CancellationToken cancellationToken = default)
    {
        LogAction("Get", url);
        return _inner.GetAsync(url, cancellationToken);
    }

    public Task<byte[]> GetByteArrayAsync(string url, CancellationToken cancellationToken = default)
    {
        LogAction("Get Bytes", url);
        return _inner.GetByteArrayAsync(url, cancellationToken);
    }

    public Task<Stream> GetStreamAsync(string url, CancellationToken cancellationToken = default)
    {
        LogAction("Get Stream", url);
        return _inner.GetStreamAsync(url, cancellationToken);
    }

    public Task<string> GetStringAsync(string url, CancellationToken cancellationToken = default)
    {
        LogAction("Get String", url);
        return _inner.GetStringAsync(url, cancellationToken);
    }

    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        LogAction(request.Method.ToString(), request.RequestUri?.ToString() ?? "null");
        return _inner.SendAsync(request, cancellationToken);
    }


    private void LogAction(string action, string url)
    {
        using var _ = _logger.Scope()
            .Add("Url", url)
            .Begin();

        _logger.LogDebug("{Action}", action);
    }
}