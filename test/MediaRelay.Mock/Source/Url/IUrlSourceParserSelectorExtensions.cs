using Moq;

namespace MediaRelay.Source.Url;

public static class IUrlSourceParserSelectorExtensions
{
    extension(IUrlSourceParserSelector)
    {
        public static IUrlSourceParserSelector Mock(Uri url, IUrlSourceParser selectedParser)
        {
            var mock = new Mock<IUrlSourceParserSelector>();

            mock.Setup(x => x.Select(url))
                .Returns(selectedParser);

            return mock.Object;
        }
    }
}
