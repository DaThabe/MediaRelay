using MediaRelay.Messaging.Envelope;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics;

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


    [TestMethod(DisplayName = "空队列等待时不应空转占用 CPU")]
    [DoNotParallelize]
    public async Task EmptyQueueWait_ShouldNotSpinCpu()
    {
        // Arrange: 出队后必须确认, 队列才真正变空
        // (只出队不确认时, 消息按约定仍留在队列里, 再次出队会立刻拿到同一条)
        var message = await _queue.DequeueAsync(TestContext.CancellationToken);
        await _queue.AcknowledgeAsync(message.Id, TestContext.CancellationToken);

        Assert.AreEqual(0, _queue.Count, "前置条件: 队列应当已空");

        // 此时 _newMessageEvent 已经被 Set 过, 若 DequeueAsync 没有把它重置, 等待就会立刻返回并空转
        using var cts = new CancellationTokenSource();
        var waiting = _queue.DequeueAsync(cts.Token).AsTask();

        // 给等待方一个真正挂起的机会
        await Task.Delay(200);

        using var process = Process.GetCurrentProcess();
        var cpuBefore = process.TotalProcessorTime;

        await Task.Delay(1000);

        var cpuUsed = process.TotalProcessorTime - cpuBefore;

        // 收尾: 取消等待, 应以 OperationCanceledException 结束
        await cts.CancelAsync();
        await Assert.ThrowsAsync<OperationCanceledException>(async () => await waiting);

        // 空转会把一个核心占满 (约 1000ms); 真正挂起则几乎不消耗 CPU
        Assert.IsTrue(cpuUsed < TimeSpan.FromMilliseconds(400),
            $"空队列等待 1000ms 期间消耗 CPU {cpuUsed.TotalMilliseconds:F0}ms, 说明没有真正挂起而是在空转");
    }


    public TestContext TestContext { get; set; } = null!;
}