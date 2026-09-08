using MediaRelay.Source;
using MediaRelay.Source.Url;
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
        var urlSourceParser = IUrlSourceParser.Mock(url, true, urlSource);
        var urlSourceParserSelector = IUrlSourceParserSelector.Mock(url, urlSourceParser);

        var sourceRelayService = ISourceRelayService.Mock(urlSource, TestContext.CancellationToken);

        var urlRelayService = new UrlRelayService(urlSourceParserSelector, sourceRelayService);
        await urlRelayService.RelayAsync(url, TestContext.CancellationToken);
    }

    public TestContext TestContext { get; set; }
}