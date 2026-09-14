using MediaRelay.Messaging.Envelope;
using Microsoft.Extensions.Logging;
using Moq;

namespace MediaRelay.Messaging.Queue.UrlMessageQueueTests;


[TestClass]
public sealed class RejectTests
{
    private readonly int _retryCount = 3;
    private readonly UrlMessage _enqueueMessage = Uri.AboutBlank;
    private UrlMessage _dequeueMessage = null!;
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
            .Returns((UrlMessage m) => UrlMessageEnvelope.Create(m, UrlMessageRetryCounter.FromCount(_retryCount)));

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
        _dequeueMessage = await _queue.DequeueAsync(TestContext.CancellationToken);
    }
    [TestCleanup]
    public async Task CleanupAsync()
    {
        await _queue.DisposeAsync();
    }



    [TestMethod]
    public async Task RejectAsync_WithRetryRemaining_ShouldRequeue()
    {
        // Act
        await _queue.RejectAsync(_dequeueMessage.Id, TestContext.CancellationToken);

        // Assert
        Assert.AreEqual(1, _queue.Count);
    }


    [TestMethod]
    public async Task RejectAsync_WhenRetryExhausted_ShouldMoveToDead()
    {
        // Act - 反复出队 + 拒绝，直到重试耗尽
        for (var i = 0; i <= _retryCount; i++)
        {
            await _queue.RejectAsync(_dequeueMessage.Id, TestContext.CancellationToken);
        }

        // Assert - 重试耗尽后，消息进入死信，队列为空
        Assert.AreEqual(0, _queue.Count);
    }


    public TestContext TestContext { get; set; } = null!;
}