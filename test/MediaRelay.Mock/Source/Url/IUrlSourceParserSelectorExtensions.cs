using Moq;

namespace MediaRelay.Source.Url;

public static class IUrlSourceParserSelectorExtensions
{
    extension(IUrlParserSelector)
    {
        public static IUrlParserSelector Mock(Uri url, IUrlSourceParser selectedParser)
        {
            var mock = new Mock<IUrlParserSelector>();

            mock.Setup(x => x.Select(url))
                .Returns(selectedParser);

            return mock.Object;
        }
    }

    extension(IUrlSourceFactory)
    {
        public static IUrlSourceFactory Mock(Uri url, IUrlSource createResult, IUrlSourceParser selectedParser)
        {
            var mock = new Mock<IUrlSourceFactory>();

            mock.Setup(x => x.Create(url))
                .Returns(createResult);

            return mock.Object;
        }
    }
}
