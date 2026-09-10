using Moq;

namespace MediaRelay.Source;

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



