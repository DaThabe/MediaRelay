using MediaRelay.Content;
using MediaRelay.Publish;
using MediaRelay.Resources;
using MediaRelay.Source;
using MediaRelay.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Cryptography;
using System.Text;

[assembly: Parallelize(Scope = ExecutionScope.MethodLevel)]


namespace MediaRelay;

public static class MockExtensions
{
    extension(Mock)
    {
        public static ILogger<T> Logger<T>()
        {
            return _loggerFactory.CreateLogger<T>();
        }
        public static IOptions<T> Options<T>(T options) where T : class
        {
            var mock = new Mock<IOptions<T>>();

            mock.Setup(x => x.Value)
                .Returns(options);

            return mock.Object;
        }


        public static Uri Uri => new("about:blank");
        public static SourceId SourceId => SourceId.Create("Test:SourceId");
        public static ResourceId ResourceId => ResourceId.Create("Test:ResourceId");
        public static ContentId ContentId => ContentId.Create("Test:ContentId");

        public static IHasher Hasher(string algorithm, Stream stream, byte[] hashResult, CancellationToken hashCts = default)
        {
            var mock = new Mock<IHasher>();

            mock.Setup(x => x.Algorithm)
                .Returns(algorithm);
            mock.Setup(x => x.HashAsync(stream, hashCts))
                .Returns(new ValueTask<byte[]>(hashResult));

            return mock.Object;
        }


        public static ISource Source(SourceId sourceId)
        {
            var mock = new Mock<ISource>();

            mock.Setup(x => x.Id)
                .Returns(sourceId);

            return mock.Object;
        }

        public static MemoryStream StringToMemoryStream(string str)
        {
            var ms = new MemoryStream();
            var bytes = Encoding.UTF8.GetBytes(str);
            ms.Write(bytes);
            ms.Position = 0;
            return ms;
        }
        public static IResource Resource(ResourceId resourceId, string extensions, Stream getStreamResult, CancellationToken getStreamCts = default)
        {
            var mock = new Mock<IResource>();

            mock.Setup(x => x.Id)
                .Returns(resourceId);
            mock.Setup(x => x.Extensions)
                .Returns(extensions);
            mock.Setup(x => x.GetStreamAsync(getStreamCts))
                .Returns(new ValueTask<Stream>(getStreamResult));

            return mock.Object;
        }

        public static IContent Content(ContentId contentId, ISource source, params IEnumerable<IResource> resources)
        {
            var mock = new Mock<IContent>();

            mock.Setup(x => x.Id)
                .Returns(contentId);
            mock.Setup(x => x.Source)
                .Returns(source);
            mock.Setup(x => x.MediaResources)
                .Returns(resources.ToHashSet());

            return mock.Object;
        }


        public static IContentExtractor ContentExtractor(ISource source, bool canExtractResult, IContent extractedContent, CancellationToken extractCts = default)
        {
            var mock = new Mock<IContentExtractor>();

            mock.Setup(x => x.CanExtract(source))
                .Returns(canExtractResult);
            mock.Setup(x => x.ExtractAsync(source, extractCts))
                .Returns(new ValueTask<IContent>(extractedContent));

            return mock.Object;
        }
        public static IContentExtractorSelector ContentExtractorSelector(ISource source, IContentExtractor selectedExtractor)
        {
            var mock = new Mock<IContentExtractorSelector>();

            mock.Setup(x => x.Select(source))
                .Returns(selectedExtractor);

            return mock.Object;
        }


        public static IPublishContentConverter PublishContentConverter(IContent content, bool canConvertResult, PublishContent convertedPublishContent)
        {
            var mock = new Mock<IPublishContentConverter>();

            mock.Setup(x => x.CanConvert(content))
                .Returns(canConvertResult);

            mock.Setup(x => x.ConvertAsync(content))
                .Returns(new ValueTask<PublishContent>(convertedPublishContent));

            return mock.Object;
        }
        public static IPublishContentConverterSelector PublishContentConverterSelector(IContent content, IPublishContentConverter selectedConverter)
        {
            var mock = new Mock<IPublishContentConverterSelector>();

            mock.Setup(x => x.Select(content))
                .Returns(selectedConverter);

            return mock.Object;
        }
        public static IPublishOrchestrator PublishOrchestrator(PublishContent publishContent)
        {
            var mock = new Mock<IPublishOrchestrator>();

            mock.Setup(x => x.PublishAsync(publishContent))
                .Returns(ValueTask.CompletedTask);

            return mock.Object;
        }
    }


    private readonly static ILoggerFactory _loggerFactory = LoggerFactory.Create(x => x.AddEmojiDebug());
}