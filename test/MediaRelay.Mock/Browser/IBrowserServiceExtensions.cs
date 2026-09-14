using MediaRelay.Content;
using Moq;

namespace MediaRelay.Browser;

public static class IBrowserServiceExtensions
{
    extension(IBrowserService)
    {
        public static IBrowserService Mock(IBrowser getSharedResult, IBrowserContext getSharedContextResult, IContent createResult, CancellationToken createAsyncCts= default)
        {
            var mock = new Mock<IBrowserService>();

            mock.Setup(x => x.GetSharedAsync())
                .Returns(new ValueTask<IBrowser>(getSharedResult));

            mock.Setup(x => x.GetSharedContextAsync())
                .Returns(new ValueTask<IBrowserContext>(getSharedContextResult));

            mock.Setup(x => x.LaunchAsync());

            return mock.Object;
        }
    }
}
