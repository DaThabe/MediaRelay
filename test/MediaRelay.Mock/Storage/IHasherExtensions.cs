using Moq;

namespace MediaRelay.Storage;

public static class IHasherExtensions
{
    extension(IHasher)
    {
        public static IHasher Mock(string algorithm, Stream stream, byte[] hashResult, CancellationToken hashCts = default)
        {
            var mock = new Mock<IHasher>();

            mock.Setup(x => x.Algorithm)
                .Returns(algorithm);
            mock.Setup(x => x.HashAsync(stream, hashCts))
                .Returns(new ValueTask<byte[]>(hashResult));

            return mock.Object;
        }
    }
}