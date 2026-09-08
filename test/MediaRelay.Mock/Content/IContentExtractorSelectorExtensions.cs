using MediaRelay.Source;
using Moq;

namespace MediaRelay.Content;

public static class IContentExtractorSelectorExtensions
{
    extension(IContentExtractorSelector)
    {
        public static IContentExtractorSelector Mock(ISource source, IContentExtractor selectedExtractor)
        {
            var mock = new Mock<IContentExtractorSelector>();

            mock.Setup(x => x.Select(source))
                .Returns(selectedExtractor);

            return mock.Object;
        }
    }
}
