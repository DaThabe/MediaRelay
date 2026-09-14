using MediaRelay.Messaging.Envelope;
using Microsoft.Extensions.Logging;
using Moq;

namespace MediaRelay.Messaging.Queue.UrlMessageQueueTests;


[TestClass]
public sealed class DequeueTests
{
    private readonly UrlMessage _enqueueMessage = Uri.AboutBlank;
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
            .Returns(true);

        // Logger
        var logger = Logger<UrlMessageQueue>.Create();

        // Queue
        _queue = new UrlMessageQueue(
            envelopePersistence: mockUrlMessageEnvelopePersistence.Object,
            envelopeCreator: mockUrlMessageEnvelopeCreator.Object,
            enqueueFilter: mockUrlMessageEnqueueFilter.Object,
            logger: logger);

        await _queue.EnqueueAsync(_enqueueMessage, TestContext.CancellationToken);
    }
    [TestCleanup]
    public async Task CleanupAsync()
    {
        await _queue.DisposeAsync();
    }


    [TestMethod(DisplayName = "出队后消息一致")]
    public async Task ShouldReturnEnqueuedMessage()
    {
        // Act
        var dequeueMessage = await _queue.DequeueAsync(TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(_enqueueMessage.Id, dequeueMessage.Id);
    }

    [TestMethod(DisplayName = "取消出队后抛出异常")]
    public async Task CancelToken_ThrowOperationCanceledException()
    {
        // Ararnge
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act + Assert
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            await _queue.DequeueAsync(cts.Token));
    }


    public TestContext TestContext { get; set; } = null!;
}