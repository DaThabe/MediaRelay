#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace System.IO;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

public static class StreamExtensions
{
    extension(Stream stream)
    {
        public void EnsureAtStart()
        {
            if (stream.CanSeek)
            {
                stream.Position = 0;
                return;
            }

            if (stream.Position != 0)
            {
                throw new InvalidOperationException($"流不可定位且当前位置为 {stream.Position}，无法从开头读取。");
            }
        }


        public async Task<MemoryStream> CopyToMemoryStreamAsync(CancellationToken cancellationToken = default)
        {
            stream.EnsureAtStart();

            if (stream.CanSeek && stream.Position != 0)
            {
                stream.Position = 0;
            }

            var ms = new MemoryStream();
            await stream.CopyToAsync(ms, cancellationToken);

            ms.Position = 0;
            return ms;
        }
    }
}