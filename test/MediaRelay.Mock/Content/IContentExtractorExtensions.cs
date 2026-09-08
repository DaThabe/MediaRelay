using MediaRelay.Source;
using Moq;

namespace MediaRelay.Content;

public static class IContentExtractorExtensions
{
    extension(IContentExtractor)
    {
        public static IContentExtractor Mock(ISource source, bool canExtractResult, IContent extractedContent, CancellationToken extractCts = default)
        {
            var mock = new Mock<IContentExtractor>();

            mock.Setup(x => x.CanExtract(source))
                .Returns(canExtractResult);
            mock.Setup(x => x.ExtractAsync(source, extractCts))
                .Returns(new ValueTask<IContent>(extractedContent));

            return mock.Object;
        }
    }
}
