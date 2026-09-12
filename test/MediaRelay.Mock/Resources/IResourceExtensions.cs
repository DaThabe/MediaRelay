using MediaRelay.Storage;
using Moq;

namespace MediaRelay.Resource;

public static class IResourceExtensions
{
    extension(IResource)
    {
        public static IResource Mock(ResourceId resourceId, MediaType mediaType, Stream getStreamResult, CancellationToken getStreamCts = default)
        {
            var mock = new Mock<IResource>();

            mock.Setup(x => x.Id)
                .Returns(resourceId);
            mock.Setup(x => x.Type)
                .Returns(mediaType);
            mock.Setup(x => x.GetStreamAsync(getStreamCts))
                .Returns(new ValueTask<Stream>(getStreamResult));

            return mock.Object;
        }
    }
}
