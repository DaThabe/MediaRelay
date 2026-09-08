using Moq;

namespace MediaRelay.Source;

public static class ISourceExtensions
{
    extension(ISource)
    {
        public static ISource Mock(SourceId sourceId)
        {
            var mock = new Mock<ISource>();

            mock.Setup(x => x.Id)
                .Returns(sourceId);

            return mock.Object;
        }
    }
}
