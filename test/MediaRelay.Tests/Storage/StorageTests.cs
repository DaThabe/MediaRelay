using Moq;
using System.Text;

namespace MediaRelay.Storage;


[TestClass]
public class StorageTests
{
    [TestMethod]
    public async Task StoreAsync_ShouldCompleteSuccessfully()
    {
        const string content = nameof(StoreAsync_ShouldCompleteSuccessfully);
        const string extension = "Test";

        // Data
        var hasher = new SHA256Hasher();
        var bytes = Encoding.UTF8.GetBytes(content);
        await using var dataStream = new MemoryStream(bytes);

        // Storage
        var storage = GetStorage(hasher);
        var info = await storage.StoreAsync(dataStream, extension, TestContext.CancellationToken);

        // Assert
        dataStream.EnsureAtStart();
        await AssertFileAndClearAsync(hasher, dataStream, extension, info, TestContext.CancellationToken);
    }

    [TestMethod]
    public async Task StoreAsync_WhenStreamIsNotSeekable_ShouldCopyToMemoryStream()
    {
        const string content = nameof(StoreAsync_WhenStreamIsNotSeekable_ShouldCopyToMemoryStream);
        const string extension = "Test";

        // Data
        var hasher = new SHA256Hasher();
        var bytes = Encoding.UTF8.GetBytes(content);
        await using var writeDataStream = new NonSeekableMemoryStream(bytes);
        await using var dataStream = new MemoryStream(bytes);

        // Act
        var storage = GetStorage(hasher);
        var info = await storage.StoreAsync(writeDataStream, extension, TestContext.CancellationToken);

        // Assert
        await AssertFileAndClearAsync(hasher, dataStream, extension, info, TestContext.CancellationToken);
    }


    private static Storage GetStorage(IHasher hasher)
    {
        // Options
        var tempFodler = Path.GetTempPath();
        var options = Mock.Options(new StorageOptions() { RootPath = tempFodler });

        // Logger
        var logger = Mock.Logger<Storage>();

        // Storage
        return new Storage(options, hasher, logger);
    }

    private static async Task AssertFileAndClearAsync(SHA256Hasher hasher, Stream content, string extension, StorageInfo info, CancellationToken cancellationToken = default)
    {
        // Data
        var filePath = info.Uri.LocalPath;
        var fileExists = File.Exists(filePath);
        var hashHex = await hasher.HashAsHexAsync(content, cancellationToken);

        // Assert
        Assert.IsTrue(fileExists, $"文件不存在: {filePath}");
        Assert.AreEqual(content.Length, info.Size, "文件大小不匹配");
        Assert.AreEqual(extension, info.Extensions, "扩展名不匹配");
        Assert.AreEqual(hasher.Algorithm, info.HashAlgorithm, "Hash算法不匹配");
        Assert.AreEqual(hashHex, info.Hash, "Hash值不匹配");

        // Clear
        File.Delete(filePath);
    }


    public TestContext TestContext { get; set; }
}