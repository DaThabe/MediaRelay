namespace MediaRelay.Http;


public interface IHttpClient
{
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
}



public static class HttpClientExtensions
{
    extension(IHttpClient httpClient)
    {
        public async Task<HttpResponseMessage> GetAsync(string url, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            return await httpClient.SendAsync(request, cancellationToken);

        }

        public async Task<string> GetStringAsync(string url, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetAsync(url, cancellationToken);
            return await response.Content.ReadAsStringAsync(cancellationToken);
        }

        public async Task<byte[]> GetByteArrayAsync(string url, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetAsync(url, cancellationToken);
            return await response.Content.ReadAsByteArrayAsync(cancellationToken);
        }

        public async Task<Stream> GetStreamAsync(string url, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetAsync(url, cancellationToken);
            return await response.Content.ReadAsStreamAsync(cancellationToken);
        }
    }
}