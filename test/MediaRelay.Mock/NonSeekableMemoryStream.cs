#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace System.IO;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public sealed class NonSeekableMemoryStream(byte[] datas) : MemoryStream(datas)
{
    public override bool CanSeek => false;
}