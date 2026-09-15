using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;

namespace MediaRelay.Browser;

[TestClass]
public sealed class PageTests
{
    private CancellationTokenSource _mockCts = null!;
    private bool _isClosed;
    private Mock<Microsoft.Playwright.IPage> _mockPage = null!;
    private ILogger<Page> _logger = null!;


    [TestInitialize]
    public void Setup()
    {
        _mockCts = new();
        _isClosed = false;

        // Playwright Page
        _mockPage = new();
        _mockPage.Setup(x => x.DisposeAsync())
            .Returns(() => { _isClosed = true; return ValueTask.CompletedTask; });
        _mockPage.Setup(x => x.IsClosed)
            .Returns(() => _isClosed);

        // Logger
        _logger = ILogger<Page>.Create();
    }
    [TestCleanup]
    public void Cleanup()
    {
        _mockCts.Cancel();
        _mockCts.Dispose();
    }



    [TestMethod(DisplayName = "取消跳转页面抛出异常(OperationCanceledException)")]
    public async Task GotoAsync_Cancel_ThrowOperationCanceledException()
    {
        // Arrange
        _mockPage.Setup(x => x.GotoAsync(It.IsAny<string>(), It.IsAny<Microsoft.Playwright.PageGotoOptions>()))
            .Returns<string, Microsoft.Playwright.PageGotoOptions>(async (_, _) =>
            {
                await Task.Delay(TimeSpan.FromMinutes(1), _mockCts.Token);

                var mockResponse = new Mock<Microsoft.Playwright.IResponse>();
                return mockResponse.Object;
            });

        // Act
        await using var page = new Page(_mockPage.Object, _logger);

        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            await page.GotoAsync("about:blank", cancellationToken: cts.Token));

        // Assert
        Assert.IsTrue(page.IsClosed);
    }

    [TestMethod(DisplayName = "取消页面脚本执行抛出异常(OperationCanceledException)")]
    public async Task EvaluateAsync_Cancel_ThrowOperationCanceledException()
    {
        // Arrange
        _mockPage.Setup(x => x.EvaluateAsync<string>(It.IsAny<string>(), It.IsAny<object>()))
            .Returns(async () =>
            {
                await Task.Delay(TimeSpan.FromMinutes(1), _mockCts.Token);
                return string.Empty;
            });

        // Act
        await using var page = new Page(_mockPage.Object, _logger);

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            await page.EvaluateAsync<string>("2+2", cancellationToken: cts.Token));

        // Assert
        Assert.IsTrue(page.IsClosed);
    }
    
    [TestMethod(DisplayName = "取消页面脚本执行抛出异常(OperationCanceledException)")]
    public async Task EvaluateAsync_T_Cancel_ThrowOperationCanceledException()
    {
        // Arrange
        _mockPage.Setup(x => x.EvaluateAsync<string>(It.IsAny<string>(), It.IsAny<object>()))
            .Returns(async () =>
            {
                await Task.Delay(TimeSpan.FromMinutes(1), _mockCts.Token);
                return string.Empty;
            });

        // Act
        await using var page = new Page(_mockPage.Object, _logger);

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            await page.EvaluateAsync<string>("2+2", cancellationToken: cts.Token));

        // Assert
        Assert.IsTrue(page.IsClosed);
    }



    public TestContext TestContext { get; set; }
}
