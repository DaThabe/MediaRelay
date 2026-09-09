using MediaRelay.Source;
using Moq;

namespace MediaRelay.Content;

public static class IContentExtractorFactoryExtensions
{
    extension(IContentExtractorFactory)
    {
        public static IContentExtractorFactory Mock(ISource source, IContent createResult, CancellationToken createAsyncCts= default)
        {
            var mock = new Mock<IContentExtractorFactory>();

            mock.Setup(x => x.CreateAsync(source, createAsyncCts))
                .Returns(new ValueTask<IContent>(createResult));

            return mock.Object;
        }
    }
}
