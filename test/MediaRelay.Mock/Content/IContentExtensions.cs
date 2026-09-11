using MediaRelay.Resources;
using MediaRelay.Source;
using Moq;

namespace MediaRelay.Content;

public static class IContentExtensions
{
    extension(IContent)
    {
        public static IContent Mock(ContentId contentId, ISource source, params IEnumerable<IResource> resources)
        {
            var mock = new Mock<IContent>();

            mock.Setup(x => x.Id)
                .Returns(contentId);
            mock.Setup(x => x.Source)
                .Returns(source);
            mock.Setup(x => x.Resources)
                .Returns(resources.ToHashSet());

            return mock.Object;
        }
    }
}