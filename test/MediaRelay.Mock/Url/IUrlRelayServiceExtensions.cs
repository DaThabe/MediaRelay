using Moq;

namespace MediaRelay.Url;


public static class IUrlRelayServiceExtensions
{
    extension(IUrlRelayService)
    {
        public static IUrlRelayService Mock(Uri url, CancellationToken relayAsyncCts = default)
        {
            var mock = new Mock<IUrlRelayService>();

            mock.Setup(x => x.RelayAsync(url, relayAsyncCts))
                .Returns(ValueTask.CompletedTask);

            return mock.Object;
        }
    }
}