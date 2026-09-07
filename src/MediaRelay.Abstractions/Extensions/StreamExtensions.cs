#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace System.IO;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

public static class StreamExtensions
{
    extension(Stream stream)
    {
        /// <inheritdoc/>
        /// <exception cref="NotSupportedException"/>
        public void EnsureAtStart()
        {
            if (stream.CanSeek)
            {
                stream.Position = 0;
                return;
            }

            throw new NotSupportedException($"流不支持定位, 无法移动至起始位");
        }


        /// <summary>
        /// 从流的当前位置开始拷贝至末尾
        /// </summary>
        public async Task<MemoryStream> CopyToMemoryStreamAsync(CancellationToken cancellationToken = default)
        {
            var ms = new MemoryStream();
            await stream.CopyToAsync(ms, cancellationToken);

            ms.Position = 0;
            return ms;
        }
    }
}