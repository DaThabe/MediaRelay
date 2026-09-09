using Moq;

namespace MediaRelay.Source.Url;

public static class IUrlSourceParserExtensions
{
    extension(IUrlSourceParser)
    {
        public static IUrlSourceParser Mock(Uri url, bool canParseResult, IUrlSource parseResult)
        {
            var mock = new Mock<IUrlSourceParser>();

            mock.Setup(x => x.CanParse(url))
                .Returns(canParseResult);
            mock.Setup(x => x.Parse(url))
                .Returns(parseResult);

            return mock.Object;
        }
    }
}
