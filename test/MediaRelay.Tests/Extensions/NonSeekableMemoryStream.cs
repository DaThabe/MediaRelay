#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace System.IO;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class IOExtensions
{
    extension(Path)
    {
        /// <summary>
        /// 创建一个临时文件路径, 并在释放后删除该文件
        /// </summary>
        public static AutoDeleteTempFile AutoDeleteTempFile() =>
            IO.AutoDeleteTempFile.Create();
    }
}


public sealed class NonSeekableMemoryStream(byte[] datas) : MemoryStream(datas)
{
    public override bool CanSeek => false;
}


public sealed class AutoDeleteTempFile : IDisposable
{
    private bool _disposed;
    private readonly string _path = Path.GetTempFileName();


    private AutoDeleteTempFile() { }
    public static AutoDeleteTempFile Create() => new();


    public static implicit operator string(AutoDeleteTempFile file)
        => file.ToString();


    public override string ToString()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return _path;
    }
    public void Dispose()
    {
        File.Delete(_path);
        _disposed = true;
    }
}