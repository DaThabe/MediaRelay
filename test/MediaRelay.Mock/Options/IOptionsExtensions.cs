using Moq;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.Options;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

public static class IOptionsExtensions
{
    extension<T>(IOptions<T>) where T : class
    {
        public static IOptions<T> Mock(T options)
        {
            var mock = new Mock<IOptions<T>>();

            mock.Setup(x => x.Value)
                .Returns(options);

            return mock.Object;
        }
    }
}
