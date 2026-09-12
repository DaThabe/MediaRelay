using MediaRelay.Source;
using MediaRelay.Url;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MediaRelay.Messaging.Queue;


[TestClass]
public sealed class UrlMessageQueueTests
{
    [TestMethod]
    public async Task EnqueueAsync_WhenCanEnqueue_ShouldEnqueue()
    {
        using var queueFileName = Path.AutoDeleteTempFile();
        var queue = await BuildAsync(queueFileName, retryCount: 0, canEnqueue: true);

        // Act
        UrlMessage enqueueMessage = Uri.AboutBlank;
        await queue.EnqueueAsync(enqueueMessage, TestContext.CancellationToken);
        var dequeueMessage = await queue.DequeueAsync(TestContext.CancellationToken);

        Assert.AreEqual(enqueueMessage.Id, dequeueMessage.Id);
    }


    [TestMethod]
    public async Task EnqueueAsync_WhenCannotEnqueue_ShouldThrow()
    {
        using var queueFileName = Path.AutoDeleteTempFile();
        var queue = await BuildAsync(queueFileName, retryCount: 0, canEnqueue: false);

        // Act
        UrlMessage enqueueMessage = Uri.AboutBlank;
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
              await queue.EnqueueAsync(enqueueMessage, TestContext.CancellationToken));
    }


    [TestMethod]
    public async Task AcknowledgeAsync_ShouldRemoveMessage()
    {
        using var queueFileName = Path.AutoDeleteTempFile();
        var queue = await BuildAsync(queueFileName, retryCount: 0, canEnqueue: true);

        // Act
        UrlMessage enqueueMessage = Uri.AboutBlank;
        await queue.EnqueueAsync(enqueueMessage, TestContext.CancellationToken);
        await queue.AcknowledgeAsync(enqueueMessage.Id, TestContext.CancellationToken);

        Assert.AreEqual(0, queue.Count);
    }

    [TestMethod]
    public async Task RejectAsync_WithRetryRemaining_ShouldRequeue()
    {
        using var queueFileName = Path.AutoDeleteTempFile();
        var queue = await BuildAsync(queueFileName, retryCount: 1, canEnqueue: true);


        // Act
        UrlMessage message = Uri.AboutBlank;
        await queue.EnqueueAsync(message, TestContext.CancellationToken);

        var dequeued = await queue.DequeueAsync(TestContext.CancellationToken);
        await queue.RejectAsync(dequeued.Id, TestContext.CancellationToken);

        // 拒绝后，消息回到队列，可以再次出队
        var requeued = await queue.DequeueAsync(TestContext.CancellationToken);
        Assert.AreEqual(message.Id, requeued.Id);
    }


    [TestMethod]
    [DataRow(3, DisplayName = "3次重试")]
    [DataRow(5, DisplayName = "5次重试")]
    [DataRow(10, DisplayName = "10次重试")]
    public async Task RejectAsync_WhenRetryExhausted_ShouldMoveToDead(int retryCount)
    {
        using var queueFileName = Path.AutoDeleteTempFile();
        var queue = await BuildAsync(queueFileName, retryCount: retryCount, canEnqueue: true);

        UrlMessage message = Uri.AboutBlank;
        await queue.EnqueueAsync(message, TestContext.CancellationToken);

        // 反复出队 + 拒绝，直到重试耗尽
        for (var i = 0; i <= retryCount; i++)
        {
            var dequeued = await queue.DequeueAsync(TestContext.CancellationToken);
            await queue.RejectAsync(dequeued.Id, TestContext.CancellationToken);
        }

        // 重试耗尽后，消息进入死信，队列为空
        Assert.AreEqual(0, queue.Count);
    }


    private async Task<UrlMessageQueue> BuildAsync(string queueFileName, int retryCount, bool canEnqueue)
    {
        await File.WriteAllTextAsync(queueFileName, "[]", TestContext.CancellationToken);

        var options = IOptions<UrlOptions>.Mock(x =>
        {
            x.MessageQueue.File = queueFileName;
            x.MessageQueue.MessageRetryCount = retryCount;
        });
        var logger = Logger<UrlMessageQueue>.Create();

        var urlSource = IUrlSource.Mock(SourceId.TestId, Uri.AboutBlank);
        var urlSourceParser = IUrlSourceParser.Mock(Uri.AboutBlank, canEnqueue, urlSource);

        return new UrlMessageQueue(options, [urlSourceParser], logger);
    }


    public TestContext TestContext { get; set; } = null!;
}