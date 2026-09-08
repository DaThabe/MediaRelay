using MediaRelay.Url;
using Moq;

namespace MediaRelay.Source.Url;

public static class IUrlSourceParserSelectorExtensions
{
    extension(IUrlParserSelector)
    {
        public static IUrlParserSelector Mock(Uri url, IUrlParser selectedParser)
        {
            var mock = new Mock<IUrlParserSelector>();

            mock.Setup(x => x.Select(url))
                .Returns(selectedParser);

            return mock.Object;
        }
    }
}
