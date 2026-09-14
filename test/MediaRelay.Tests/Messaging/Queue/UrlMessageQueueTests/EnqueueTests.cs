using MediaRelay.Messaging.Envelope;
using Microsoft.Extensions.Logging;
using Moq;

namespace MediaRelay.Messaging.Queue.UrlMessageQueueTests;


[TestClass]
public sealed class EnqueueTests
{
    private string queueFilePath = null!;
    private bool _canEnqueue;
    private UrlMessageQueue _queue = null!;


    [TestInitialize]
    public async Task SetupAsync()
    {
        // EnvelopePersistence
        var mockUrlMessageEnvelopePersistence = new Mock<IUrlMessageEnvelopePersistence>();

        // EnvelopeCreator
        var mockUrlMessageEnvelopeCreator = new Mock<IUrlMessageEnvelopeCreator>();
        mockUrlMessageEnvelopeCreator.Setup(x => x.CanCreate(It.IsAny<UrlMessage>())).Returns(true);
        mockUrlMessageEnvelopeCreator.Setup(x => x.Create(It.IsAny<UrlMessage>()))
            .Returns((UrlMessage m) => UrlMessageEnvelope.Create(m));

        // EnqueueFilter
        var mockUrlMessageEnqueueFilter = new Mock<IUrlMessageEnqueueFilter>();
        mockUrlMessageEnqueueFilter.Setup(x => x.CanEnqueue(It.IsAny<UrlMessage>()))
            .Returns(() => _canEnqueue);

        // Logger
        var logger = Logger<UrlMessageQueue>.Create();

        // Queue
        _queue = new UrlMessageQueue(
            envelopePersistence: mockUrlMessageEnvelopePersistence.Object,
            envelopeCreator: mockUrlMessageEnvelopeCreator.Object,
            enqueueFilter: mockUrlMessageEnqueueFilter.Object,
            logger: logger);
    }
    [TestCleanup]
    public async Task CleanupAsync()
    {
        await _queue.DisposeAsync();
        if (File.Exists(queueFilePath)) File.Delete(queueFilePath);
    }




    [TestMethod(DisplayName = "正常入队")]
    public async Task ShouldEnqueue()
    {
        // Ararnge
        _canEnqueue = true;

        // Act
        await _queue.EnqueueAsync(Uri.AboutBlank, TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, _queue.Count);
    }

    [TestMethod(DisplayName = "不支持的消息抛出异常(InvalidOperationException)")]
    public async Task UnsupportedMessage_ShouldThrowInvalidOperationException()
    {
        //Ararnge
        _canEnqueue = false;

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _queue.EnqueueAsync(Uri.AboutBlank, TestContext.CancellationToken));

        // Assert
        Assert.AreEqual(0, _queue.Count);
    }

    [TestMethod(DisplayName = "空消息抛出异常(ArgumentNullException)")]
    public async Task NullMessage_ShouldThrowArgumentNullException()
    {
        // Act  
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
           await _queue.EnqueueAsync(null!, TestContext.CancellationToken));

        // Assert
        Assert.AreEqual(0, _queue.Count);
    }

    [TestMethod(DisplayName = "取消入队后抛出异常(OperationCanceledException)")]
    public async Task CanceledToken_ShouldThrowOperationCanceledException()
    {
        // Ararnge
        _canEnqueue = true;

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act + Assert
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            await _queue.EnqueueAsync(Uri.AboutBlank, cts.Token));
    }

    public TestContext TestContext { get; set; } = null!;
}