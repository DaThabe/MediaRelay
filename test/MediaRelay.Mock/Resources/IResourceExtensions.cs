using Moq;

namespace MediaRelay.Resources;

public static class IResourceExtensions
{
    extension(IResource)
    {
        public static IResource Mock(ResourceId resourceId, string extensions, Stream getStreamResult, CancellationToken getStreamCts = default)
        {
            var mock = new Mock<IResource>();

            mock.Setup(x => x.Id)
                .Returns(resourceId);
            mock.Setup(x => x.Extensions)
                .Returns(extensions);
            mock.Setup(x => x.GetStreamAsync(getStreamCts))
                .Returns(new ValueTask<Stream>(getStreamResult));

            return mock.Object;
        }
    }
}
