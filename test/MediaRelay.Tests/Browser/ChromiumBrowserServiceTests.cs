using MediaRelay.Playwright;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace MediaRelay.Browser;

[TestClass]
public sealed class ChromiumBrowserServiceTests
{
    [TestMethod]
    public async Task GetSharedAsync_WhenCalledConcurrently_ShouldLaunchOnce()
    {
        for (var i = 0; i < 100; i++)
        {
            // Arrange
            var mockBrowser = new Mock<Microsoft.Playwright.IBrowser>();

            var mockPlaywright = new Mock<Microsoft.Playwright.IPlaywright>();
            mockPlaywright.Setup(x => x.Chromium.LaunchAsync(It.IsAny<Microsoft.Playwright.BrowserTypeLaunchOptions>()))
                .Returns(Task.FromResult(mockBrowser.Object));

            var mockPlaywrightService = new Mock<IPlaywrightService>();
            mockPlaywrightService.Setup(x => x.GetPlaywrightAsync())
                .Returns(new ValueTask<Microsoft.Playwright.IPlaywright>(mockPlaywright.Object));


            var options = IOptions<BrowserOptions>.Mock();
            var logger = ILogger<ChromiumBrowserService>.Create();

            var chromiumBrowserService = new ChromiumBrowserService(mockPlaywrightService.Object, options, logger);

            // Act
            mockPlaywright.Invocations.Clear();

            var browsers = await Task.WhenAll(
                chromiumBrowserService.GetSharedAsync().AsTask(),
                chromiumBrowserService.GetSharedAsync().AsTask(),
                chromiumBrowserService.GetSharedAsync().AsTask(),
                chromiumBrowserService.GetSharedAsync().AsTask(),
                chromiumBrowserService.GetSharedAsync().AsTask(),
                chromiumBrowserService.GetSharedAsync().AsTask());

            // Assert
            mockPlaywright.Verify(x => x.Chromium.LaunchAsync(It.IsAny<Microsoft.Playwright.BrowserTypeLaunchOptions>()), Times.Once());
        }
    }
}
