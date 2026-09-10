using MediaRelay.Url;
using Moq;

namespace MediaRelay.Source;

public static class IUrlSourceExtensions
{
    extension(IUrlSource)
    {
        public static IUrlSource Mock(SourceId sourceId, Uri url)
        {
            var mock = new Mock<IUrlSource>();

            mock.Setup(x => x.Id)
                .Returns(sourceId);
            mock.Setup(x => x.Url)
                .Returns(url);

            return mock.Object;
        }
    }
}
