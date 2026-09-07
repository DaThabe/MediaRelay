using Moq;
using System.Security.Cryptography;

namespace MediaRelay.Storage;

[TestClass]
public class StorageTests
{
    [TestMethod]
    public async Task StoreAsync()
    {
        var tempFodler = Path.GetTempPath();
        var options = Mock.Options(new StorageOptions() { RootPath = tempFodler });
        var logger = Mock.Logger<Storage>();

        // Data
        var data = "HelloWorld";
        var stream = Mock.StringToMemoryStream(data);
        const string extension = "Test";

        // Hash
        const string hashAlgorithm = "SHA256";
        byte[] hashResult = SHA256.HashData(stream);
        var hasher = Mock.Hasher(hashAlgorithm, stream, hashResult, TestContext.CancellationToken);

        // Storage
        var storage = new Storage(options, hasher, logger);
        var info = await storage.StoreAsync(stream, extension, TestContext.CancellationToken);

        // Assert
        Assert.IsNotNull(info);
        Assert.IsNotNull(info.Uri);
        Assert.AreEqual(extension, info.Extensions);
        Assert.AreEqual(Convert.ToHexString(hashResult).ToLowerInvariant(), info.Hash);
        Assert.AreEqual(hashAlgorithm, info.HashAlgorithm);
        Assert.IsGreaterThan(0, info.Size);

        // 文件
        var filePath = info.Uri.LocalPath;
        Assert.IsTrue(File.Exists(filePath), $"文件不存在: {filePath}");
        // 验证
        var savedContent = await File.ReadAllTextAsync(filePath, TestContext.CancellationToken);
        Assert.AreEqual(data, savedContent);
        // 清理
        File.Delete(filePath);
    }


    public TestContext TestContext { get; set; }
}
