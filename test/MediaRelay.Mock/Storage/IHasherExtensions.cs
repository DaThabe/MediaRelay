using Moq;

namespace MediaRelay.Storage;

public static class IHasherExtensions
{
    extension(IHasher)
    {
        public static IHasher Mock(Stream stream, HashInfo hashAsyncResult, CancellationToken hashCts = default)
        {
            var mock = new Mock<IHasher>();

            mock.Setup(x => x.HashAsync(stream, hashCts))
                .Returns(new ValueTask<HashInfo>(hashAsyncResult));

            return mock.Object;
        }
    }
}