using MediaRelay.Messaging;
using MediaRelay.Messaging.Envelope;
using MediaRelay.Messaging.Queue;
using MediaRelay.Source;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections;

namespace MediaRelay;


[TestClass]
public class UrlRelayServiceTest
{
    private UrlRelayService _urlRelayService = null!;


    [TestInitialize]
    public async Task SetupAsync()
    {
        // UrlSource
        var mockUrlSource = new Mock<IUrlSource>();

        // UrlSourceFactory
        var mockUrlSourceFactory = new Mock<IUrlSourceFactory>();
        mockUrlSourceFactory.Setup(x => x.Create(It.IsAny<Uri>()))
            .Returns(mockUrlSource.Object);

        // SourceRelayService
        var mockSourceRelayService = new Mock<ISourceRelayService>();

        // Logger
        var logger = Logger<UrlMessageQueue>.Create();

        // Queue
        _urlRelayService = new UrlRelayService(mockUrlSourceFactory.Object, mockSourceRelayService.Object);
    }



    [TestMethod(DisplayName = "正常转发网址")]
    public async Task RelayAsync_ShouldCompleteSuccessfully()
    {
        // Arrange
        var url = Uri.TestHttpsUrl;

        // Act
        await _urlRelayService.RelayAsync(url, TestContext.CancellationToken);
    }


    public TestContext TestContext { get; set; } = null!;
}