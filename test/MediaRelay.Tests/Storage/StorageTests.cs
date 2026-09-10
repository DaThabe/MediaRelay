using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;

namespace MediaRelay.Storage;


[TestClass]
public class StorageTests
{
    [TestMethod]
    public async Task StoreAsync_ShouldCompleteSuccessfully()
    {
        const string content = nameof(StoreAsync_ShouldCompleteSuccessfully);
        var mediaType = MediaType.Png;

        // Data
        var hasher = new SHA256Hasher();
        var bytes = Encoding.UTF8.GetBytes(content);
        await using var dataStream = new MemoryStream(bytes);

        // Storage
        var storage = GetStorage(hasher);
        var info = await storage.StoreAsync(dataStream, mediaType, TestContext.CancellationToken);

        // Assert
        dataStream.EnsureAtStart();
        await AssertFileAndClearAsync(hasher, dataStream, mediaType, info, TestContext.CancellationToken);
    }

    [TestMethod]
    public async Task StoreAsync_WhenStreamIsNotSeekable_ShouldCopyToMemoryStream()
    {
        const string content = nameof(StoreAsync_WhenStreamIsNotSeekable_ShouldCopyToMemoryStream);
        var mediaType = MediaType.Png;

        // Data
        var hasher = new SHA256Hasher();
        var bytes = Encoding.UTF8.GetBytes(content);
        await using var writeDataStream = new NonSeekableMemoryStream(bytes);
        await using var dataStream = new MemoryStream(bytes);

        // Act
        var storage = GetStorage(hasher);
        var info = await storage.StoreAsync(writeDataStream, mediaType, TestContext.CancellationToken);

        // Assert
        await AssertFileAndClearAsync(hasher, dataStream, mediaType, info, TestContext.CancellationToken);
    }


    private static Storage GetStorage(IHasher hasher)
    {
        // Options
        var tempFodler = Path.GetTempPath();
        var options = IOptions<StorageOptions>.Mock(x => x.RootPath = tempFodler);

        // Logger
        var logger = ILogger<Storage>.Create();

        // Storage
        return new Storage(options, hasher, logger);
    }

    private static async Task AssertFileAndClearAsync(SHA256Hasher hasher, Stream content, MediaType mediaType, StorageInfo info, CancellationToken cancellationToken = default)
    {
        // Data
        var filePath = info.Uri.LocalPath;
        var fileExists = File.Exists(filePath);
        var hashInfo = await hasher.HashAsync(content, cancellationToken);

        // Assert
        Assert.IsTrue(fileExists, $"文件不存在: {filePath}");
        Assert.AreEqual(content.Length, info.Size, "文件大小不匹配");
        Assert.AreEqual(mediaType, info.MediaType, "媒体类型不匹配");
        Assert.AreEqual(hashInfo, info.HashInfo, "Hash信息不匹配");

        // Clear
        File.Delete(filePath);
    }


    public TestContext TestContext { get; set; }
}