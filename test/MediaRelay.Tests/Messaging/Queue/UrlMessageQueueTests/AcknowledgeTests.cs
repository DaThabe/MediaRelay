using MediaRelay.Messaging.Envelope;
using Microsoft.Extensions.Logging;
using Moq;

namespace MediaRelay.Messaging.Queue.UrlMessageQueueTests;


[TestClass]
public sealed class AcknowledgeTests
{
    private readonly UrlMessage _queuedMessage = Uri.AboutBlank;
    private UrlMessage _dequeuedMessage = Uri.AboutBlank;
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

        await _queue.EnqueueAsync(_queuedMessage, TestContext.CancellationToken);
        _dequeuedMessage = await _queue.DequeueAsync(TestContext.CancellationToken);
    }
    [TestCleanup]
    public async Task CleanupAsync()
    {
        await _queue.DisposeAsync();
    }


    [TestMethod]
    public async Task Acknowledge_ShouldRemoveMessage()
    {
        // Act
        await _queue.AcknowledgeAsync(_dequeuedMessage.Id, TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(0, _queue.Count);
    }

    [TestMethod]
    public async Task Cancel_ThrowOperationCanceledException()
    {
        // Ararnge
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act + Assert
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            await _queue.AcknowledgeAsync(_dequeuedMessage.Id, cts.Token));
    }

    public TestContext TestContext { get; set; } = null!;
}