using System.Text;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace System;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

public static class SystemExtensions
{
    extension(string str)
    {
        public MemoryStream ToMemoryStream(Encoding encoding)
        {
            var ms = new MemoryStream();
            var bytes = encoding.GetBytes(str);
            ms.Write(bytes);
            ms.Position = 0;
            return ms;
        }

        public MemoryStream ToMemoryStreamUTF8() => str.ToMemoryStream(Encoding.UTF8);
    }


    extension(Uri)
    {
        public static Uri AboutBlank => new("about:blank");
        public static Uri MockHttp => new("http://mock.mediarelay.com/");
        public static Uri MockHttps => new("http://mock.mediarelay.com/");
    }
}