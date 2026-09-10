using MediaRelay.Source;
using MediaRelay.Url;

namespace MediaRelay;


[TestClass]
public class UrlRelayServiceTest
{
    [TestMethod]
    public async Task RelayAsync_ShouldCompleteSuccessfully()
    {
        var url = Uri.MockHttps;

        var urlSource = IUrlSource.Mock(SourceId.Test, url);
        var urlSourceFactory = IUrlSourceFactory.Mock(url, urlSource);

        var sourceRelayService = ISourceRelayService.Mock(urlSource, TestContext.CancellationToken);

        var urlRelayService = new UrlRelayService(urlSourceFactory, sourceRelayService);
        await urlRelayService.RelayAsync(url, TestContext.CancellationToken);
    }

    public TestContext TestContext { get; set; }
}