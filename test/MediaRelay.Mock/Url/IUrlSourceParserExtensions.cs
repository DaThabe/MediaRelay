using MediaRelay.Source;
using Moq;

namespace MediaRelay.Url;


public static class IUrlSourceParserExtensions
{
    extension(IUrlSourceParser)
    {
        public static IUrlSourceParser Mock(Uri uri, bool canParseResult, IUrlSource parseResult)
        {
            var mock = new Mock<IUrlSourceParser>();

            mock.Setup(x => x.CanParse(uri))
                .Returns(canParseResult);
            mock.Setup(x => x.Parse(uri))
                .Returns(parseResult);

            return mock.Object;
        }
    }
}