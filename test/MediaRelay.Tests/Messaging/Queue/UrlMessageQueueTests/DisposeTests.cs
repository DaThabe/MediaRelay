using MediaRelay.Messaging.Envelope;
using Microsoft.Extensions.Logging;
using Moq;

namespace MediaRelay.Messaging.Queue.UrlMessageQueueTests;


[TestClass]
public sealed class DisposeTests
{
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
        await _queue.DisposeAsync();
    }


    [TestMethod]
    public async Task EnqueueShouldThrowObjectDisposedException()
    {
        // Act + Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(async () =>
            await _queue.EnqueueAsync(Uri.AboutBlank, TestContext.CancellationToken));
    }

    [TestMethod]
    public async Task DequeueShouldThrowObjectDisposedException()
    {
        // Act + Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(async () =>
            await _queue.DequeueAsync(TestContext.CancellationToken));
    }

    [TestMethod]
    public async Task AcknowledgeShouldThrowObjectDisposedException()
    {
        // Arrange
        UrlMessage message = Uri.AboutBlank;

        // Act + Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(async () =>
            await _queue.AcknowledgeAsync(message.Id, TestContext.CancellationToken));
    }

    [TestMethod]
    public async Task RejectShouldThrowObjectDisposedException()
    {
        // Arrange
        UrlMessage message = Uri.AboutBlank;

        // Act + Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(async () =>
            await _queue.RejectAsync(message.Id, TestContext.CancellationToken));
    }

    [TestMethod]
    public async Task DisposeAgainShouldNotThrow()
    {
        // Act + Assert
        await _queue.DisposeAsync();
    }


    public TestContext TestContext { get; set; } = null!;
}