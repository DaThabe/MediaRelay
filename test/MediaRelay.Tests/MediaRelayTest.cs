using MediaRelay.Publish;
using MediaRelay.Storage;
using Moq;

namespace MediaRelay;


[TestClass]
public class MediaRelayTest
{
    [TestMethod]
    public async Task HandleAsync_ShouldCompleteSuccessfully()
    {
        // Data
        var url = Mock.Uri;
        var source = Mock.Source(Mock.SourceId);
        var resource = Mock.Resource(Mock.ResourceId, "test", Mock.StringToMemoryStream("TestData"), TestContext.CancellationToken);
        var content = Mock.Content(Mock.ContentId, source, resource);
        var publishContent = new PublishContent()
        {
            ContentId = content.Id,
            Resources = new HashSet<StorageInfo>()
            {
                 new() { Extensions = "test", Hash = "123456", HashAlgorithm = "Test", Size = 1024, Uri = url }
            },
            SourceUrl = Mock.Uri
        };

        // Arg1
        var contentExtractor = Mock.ContentExtractor(source, true, content, TestContext.CancellationToken);
        var contentExtractorSelector = Mock.ContentExtractorSelector(source, contentExtractor);
        // Arg2
        var publishContentConverter = Mock.PublishContentConverter(content, true, publishContent);
        var publishContentConverterSelector = Mock.PublishContentConverterSelector(content, publishContentConverter);
        // Arg3
        var publishOrchestrator = Mock.PublishOrchestrator(publishContent);
        // Arg4
        var logger = Mock.Logger<MediaRelay>();

        var mediaRelay = new MediaRelay(contentExtractorSelector, publishContentConverterSelector, publishOrchestrator, logger);
        await mediaRelay.HandleAsync(source, TestContext.CancellationToken);
    }

    public TestContext TestContext { get; set; }
}