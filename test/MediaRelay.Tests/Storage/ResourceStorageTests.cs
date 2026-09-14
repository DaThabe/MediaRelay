using MediaRelay.Resource;
using MediaRelay.Storage.Hash;
using Microsoft.Extensions.Logging;
using Moq;

namespace MediaRelay.Storage;


[TestClass]
public class ResourceStorageTests
{
    private ResourceStorage _resourceStorage = null!;


    [TestInitialize]
    public async Task SetupAsync()
    {
        var storageInfo = new StorageInfo()
        {
            HashInfo = HashInfo.FromSHA256([0, 1, 2, 3, 4, 5, 6]),
            MediaType = MediaType.Empty,
            Size = 0,
            Uri = Uri.Empty
        };

        // Storage
        var mockStorage = new Mock<IStorage>();
        mockStorage.Setup(x => x.StoreAsync(It.IsAny<Stream>(), It.IsAny<MediaType>(), It.IsAny<CancellationToken>()))
            .Returns(new ValueTask<StorageInfo>(storageInfo));

        // Logger
        var logger = Logger<ResourceStorage>.Create();

        // ResourceStorage
        _resourceStorage = new ResourceStorage(mockStorage.Object, logger);
    }



    [TestMethod(DisplayName = "正常储存所有资源")]
    public async Task StoreAllAsync_ShouldCompleteSuccessfully()
    {
        // Arrange
        var mockResource1 = new Mock<IResource>();
        mockResource1.Setup(x => x.Id).Returns(ResourceId.Create("1"));

        var mockResource2 = new Mock<IResource>();
        mockResource2.Setup(x => x.Id).Returns(ResourceId.Create("2"));

        var mockResource3 = new Mock<IResource>();
        mockResource3.Setup(x => x.Id).Returns(ResourceId.Create("3"));

        var resources = new IResource[]
        {
            mockResource1.Object,
            mockResource2.Object,
            mockResource3.Object,
        };

        // Act
        var storageResources = await _resourceStorage
            .StoreAllAsync(resources, TestContext.CancellationToken);

        // Assert
        CollectionAssert.AreEquivalent(
            resources.Select(x => x.Id).ToArray(),
            storageResources.Select(x => x.Key).ToArray());
    }

    [TestMethod(DisplayName = "储存了资源空集合，返回空结果")]
    public async Task StoreAllAsync_WhenNoResources_ShouldReturnEmpty()
    {
        // Act
        var result = await _resourceStorage
            .StoreAllAsync(null!, TestContext.CancellationToken);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result);

        Assert.HasCount(0, result);
    }

    [TestMethod(DisplayName = "储存所有资源，其中一个抛异常，中断并抛出")]
    public async Task StoreAllAsync_ResourceInnerException_StopStore_ThrowInnerException()
    {
        // Arrange
        using var cts = new CancellationTokenSource();

        var mockStream = new Mock<Stream>();

        var resource1 = new Mock<IResource>();
        resource1.Setup(x => x.Id).Returns(ResourceId.Create("1"));
        resource1.Setup(x => x.GetStreamAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockStream.Object);

        var resource2 = new Mock<IResource>();
        resource2.Setup(x => x.Id).Returns(ResourceId.Create("2"));
        resource2.Setup(x => x.GetStreamAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("模拟失败"));

        var resource3 = new Mock<IResource>();
        resource3.Setup(x => x.Id).Returns(ResourceId.Create("3"));

        // Act
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _resourceStorage.StoreAllAsync([resource1.Object, resource2.Object, resource3.Object], cts.Token));

        // Assert
        Assert.IsInstanceOfType<InvalidOperationException>(ex);
    }

    [TestMethod(DisplayName = "储存所有资源，如果取消了则直接放弃储存")]
    public async Task StoreAllAsync_WhenCanceledDuringProcessing_ShouldAbortRemaining()
    {
        // Arrange
        using var cts = new CancellationTokenSource();

        var mockStream = new Mock<Stream>();

        var resource1 = new Mock<IResource>();
        resource1.Setup(x => x.Id).Returns(ResourceId.Create("1"));
        resource1.Setup(x => x.GetStreamAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockStream.Object);

        var resource2 = new Mock<IResource>();
        resource2.Setup(x => x.Id).Returns(ResourceId.Create("2"));
        resource2.Setup(x => x.GetStreamAsync(It.IsAny<CancellationToken>()))
            .Returns(async (CancellationToken ct) =>
            {
                await cts.CancelAsync();
                ct.ThrowIfCancellationRequested();

                return mockStream.Object;
            });

        var resource3 = new Mock<IResource>();
        resource3.Setup(x => x.Id).Returns(ResourceId.Create("3"));

        // Act
        var ex = await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            await _resourceStorage.StoreAllAsync([resource1.Object, resource2.Object, resource3.Object], cts.Token));

        // Assert
        Assert.IsInstanceOfType<OperationCanceledException>(ex);
    }



    public TestContext TestContext { get; set; }
}