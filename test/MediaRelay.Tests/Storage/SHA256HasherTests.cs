using System.Security.Cryptography;
using System.Text;

namespace MediaRelay.Storage;

[TestClass]
public class SHA256HasherTests
{
    [TestMethod]
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

    [TestMethod]
    public async Task HashAsync_NonSeekableStream_ThrowsInvalidOperationException()
    {
        // Data
        const string content = nameof(HashAsync_NonSeekableStream_ThrowsInvalidOperationException);
        var bytes = Encoding.UTF8.GetBytes(content);
        await using var dataStream = new NonSeekableMemoryStream(bytes);

        // Assert
        var hasher = new SHA256Hasher();
        var ex = await Assert.ThrowsAsync<NotSupportedException>(async () =>
            await hasher.HashAsync(dataStream, TestContext.CancellationToken));

        System.Console.WriteLine(ex);
    }


    [TestMethod]
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