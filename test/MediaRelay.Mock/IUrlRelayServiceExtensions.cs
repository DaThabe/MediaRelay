using MediaRelay.Source;
using MediaRelay.Url;
using Moq;

namespace MediaRelay;

public static class IUrlRelayServiceExtensions
{
    extension(IUrlRelay)
    {
        public static IUrlRelay Mock(Uri url, CancellationToken relayAsyncCts = default)
        {
            var mock = new Mock<IUrlRelay>();

            mock.Setup(x => x.RelayAsync(url, relayAsyncCts))
                .Returns(ValueTask.CompletedTask);

            return mock.Object;
        }
    }
}

public static class ISourceRelayServiceExtensions
{
    extension(ISourceRelayService)
    {
        public static ISourceRelayService Mock(ISource source, CancellationToken relayAsyncCts = default)
        {
            var mock = new Mock<ISourceRelayService>();

            mock.Setup(x => x.RelayAsync(source, relayAsyncCts))
                .Returns(ValueTask.CompletedTask);

            return mock.Object;
        }
    }
}