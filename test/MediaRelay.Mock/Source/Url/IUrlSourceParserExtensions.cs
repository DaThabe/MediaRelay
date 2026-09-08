using MediaRelay.Url;
using Moq;

namespace MediaRelay.Source.Url;

public static class IUrlSourceParserExtensions
{
    extension(IUrlParser)
    {
        public static IUrlParser Mock(Uri url, bool canParseResult, IUrlSource parseResult)
        {
            var mock = new Mock<IUrlParser>();

            mock.Setup(x => x.CanParse(url))
                .Returns(canParseResult);
            mock.Setup(x => x.Parse(url))
                .Returns(parseResult);

            return mock.Object;
        }
    }
}
