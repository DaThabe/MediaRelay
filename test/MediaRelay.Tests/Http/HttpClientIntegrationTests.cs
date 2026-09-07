using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

namespace MediaRelay.Http;


[TestClass]
public sealed class HttpClientIntegrationTests
{
    private HttpClient _client = null!;

    [TestInitialize]
    public void Initialize()
    {
        var options = new HttpOptions
        {
            IgnoreSslErrors = true,
            Timeout = TimeSpan.FromSeconds(30),
            UserAgent = "TestAgent"
        };

        var mockOptions = Mock.Of<IOptions<HttpOptions>>(x => x.Value == options);
        var mockLogger = NullLogger<HttpClient>.Instance;

        _client = new HttpClient(mockOptions, mockLogger);
    }


    [TestMethod]
    public async Task SendPixivArtworksRequestAsync()
    {
        //var request = new HttpRequestMessage(HttpMethod.Get, "https://i.pximg.net/img-original/img/2026/03/27/15/36/58/142800168_p0.png");
        //request.Headers.Referrer = new Uri("https://www.pixiv.net/");

        //var response = await _client.SendAsync(request, TestContext.CancellationToken);
        //await using var stream = await response.Content.ReadAsStreamAsync(TestContext.CancellationToken);

        //Assert.IsLessThan(stream.Length, 0, "没有响应数据");
    }


    public TestContext TestContext { get; set; }
}
