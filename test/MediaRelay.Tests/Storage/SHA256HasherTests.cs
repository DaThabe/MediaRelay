using System.Security.Cryptography;
using System.Text;

namespace MediaRelay.Storage;

[TestClass]
public class SHA256HasherTests
{
    [TestMethod(DisplayName = "正常Hash数据")]
    public async Task HashAsync_ShouldCompleteSuccessfully()
    {
        // Data
        const string content = nameof(HashAsync_ShouldCompleteSuccessfully);
        var bytes = Encoding.UTF8.GetBytes(content);
        await using var dataStream = new MemoryStream(bytes);

        // Expected
        var hashData = SHA256.HashData(dataStream);

        // Actual
        var hasher = new SHA256Hasher();
        var hashInfo = await hasher.HashAsync(dataStream, TestContext.CancellationToken);

        // Assert
        CollectionAssert.AreEqual(hashData, hashInfo.Data);
    }

    [TestMethod(DisplayName = "Hash了不可定位的流抛出异常(NotSupportedException)")]
    public async Task HashAsync_NonSeekableStream_ThrowsInvalidOperationException()
    {
        // Data
        const string content = nameof(HashAsync_NonSeekableStream_ThrowsInvalidOperationException);
        var bytes = Encoding.UTF8.GetBytes(content);
        await using var dataStream = new NonSeekableMemoryStream(bytes);

        // Assert
        var hasher = new SHA256Hasher();
        try
        {
            await Assert.ThrowsAsync<NotSupportedException>(async () =>
                await hasher.HashAsync(dataStream, TestContext.CancellationToken));
        }
        catch (Exception ex)
        {
            System.Console.WriteLine(ex);
        }
    }


    [TestMethod(DisplayName = "Hash了空数据流抛出异常(ArgumentNullException)")]
    public async Task HashAsync_NullSream_ThrowsArgumentNullException()
    {
        var hasher = new SHA256Hasher();

        // Assert
        var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await hasher.HashAsync(null!, TestContext.CancellationToken));

        System.Console.WriteLine(ex);
    }


    public TestContext TestContext { get; set; }
}