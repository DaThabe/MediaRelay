#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace System;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class UriExtensions
{
    extension(Uri)
    {
        public static Uri AboutBlank => new("about:blank");
        public static Uri TestHttpUrl => new("http://mock.mediarelay.com/");
        public static Uri TestHttpsUrl => new("http://mock.mediarelay.com/");
    }
}