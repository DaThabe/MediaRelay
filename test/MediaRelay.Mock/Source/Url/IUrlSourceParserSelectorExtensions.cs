using Moq;

namespace MediaRelay.Source.Url;

public static class IUrlSourceParserSelectorExtensions
{
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
