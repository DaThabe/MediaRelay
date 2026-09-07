using MediaRelay.Resources;
using Moq;

namespace MediaRelay.Storage;


[TestClass]
public class ResourceStorageTests
{
    [TestMethod]
    public async Task StoreAllAsync_ShouldCompleteSuccessfully()
    {
        // Data
        var resources = new IResource[]
        {
            Mock.Resource(ResourceId.Create("1"), "Ext", Mock.StringToMemoryStream("HelloWorld1!"), TestContext.CancellationToken),
            Mock.Resource(ResourceId.Create("2"), "Ext", Mock.StringToMemoryStream("HelloWorld2!"), TestContext.CancellationToken),
            Mock.Resource(ResourceId.Create("3"), "Ext", Mock.StringToMemoryStream("HelloWorld3!"), TestContext.CancellationToken)
        };

        // Assert
        var resourceStorage = GetResourceStorage();
        var storageResources = await resourceStorage
            .StoreAllAsync(resources, TestContext.CancellationToken);

        CollectionAssert.AreEquivalent(
            resources.Select(x => x.Id).ToArray(),
            storageResources.Select(x => x.Key).ToArray());

        // Clean
        CleanFiles(storageResources.Select(x => x.Value.Uri.LocalPath).ToList());
    }

    [TestMethod]
    public async Task StoreAllAsync_WhenOneResourceFails_ShouldStoreOthers()
    {
        // Resource
        var mockExceptionResource = new Mock<IResource>();
        mockExceptionResource.Setup(x => x.Id).Returns(ResourceId.Create("2"));
        mockExceptionResource.Setup(x => x.Extensions).Returns("Ext");

        static ValueTask<Stream> GetStreamWithException() => throw new InvalidOperationException("我是故意失败的");
        mockExceptionResource.Setup(x => x.GetStreamAsync(TestContext.CancellationToken))
            .Returns(GetStreamWithException);

        var resource1 = Mock.Resource(ResourceId.Create("1"), "Ext", Mock.StringToMemoryStream("HelloWorld1!"), TestContext.CancellationToken);
        var resource2 = mockExceptionResource.Object;
        var resource3 = Mock.Resource(ResourceId.Create("3"), "Ext", Mock.StringToMemoryStream("HelloWorld3!"), TestContext.CancellationToken);

        // Data
        IResource[] allResources = [resource1, resource2, resource3];
        IResource[] successResources = [resource1, resource3];

        // Assert
        var resourceStorage = GetResourceStorage();
        var storageResources = await resourceStorage
            .StoreAllAsync(allResources, TestContext.CancellationToken);

        CollectionAssert.AreEquivalent(
            successResources.Select(x => x.Id).ToArray(),
            storageResources.Select(x => x.Key).ToArray());

        // Clean
        CleanFiles(storageResources.Select(x => x.Value.Uri.LocalPath).ToList());
    }

    [TestMethod]
    public async Task StoreAllAsync_WhenNoResources_ShouldReturnEmpty()
    {
        var resourceStorage = GetResourceStorage();

        // Assert
        var result1 = await resourceStorage
            .StoreAllAsync(null!, TestContext.CancellationToken);

        var result2 = await resourceStorage
            .StoreAllAsync([], TestContext.CancellationToken);

        Assert.IsNotNull(result1);
        Assert.IsEmpty(result1);

        Assert.IsNotNull(result2);
        Assert.IsEmpty(result2);

        Assert.HasCount(result1.Count, result2);
    }


    private static ResourceStorage GetResourceStorage()
    {
        // Storage
        var tempFodler = Path.GetTempPath();
        var options = Mock.Options(new StorageOptions() { RootPath = tempFodler });
        var hasher = new SHA256Hasher();
        var storageLogger = Mock.Logger<Storage>();
        var storage = new Storage(options, hasher, storageLogger);


        var resourceStorageLogger = Mock.Logger<ResourceStorage>();
        return new ResourceStorage(storage, resourceStorageLogger);
    }

    private static void CleanFiles(List<string> filePaths)
    {
        filePaths.ForEach(path =>
        {
            if (!File.Exists(path)) return;

            File.Delete(path);
            System.Console.WriteLine($"文件已清理: {path}");
        });
    }


    public TestContext TestContext { get; set; }
}