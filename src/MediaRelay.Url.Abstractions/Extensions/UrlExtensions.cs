using System.Collections.Frozen;
using System.Collections.Specialized;
using System.Web;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace System;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class UrlExtensions
{
    extension(string url)
    {
        public string CombineUrl(params string[] segments)
        {
            var uri = url.TrimEnd('/');
            foreach (var segment in segments)
            {
                uri = $"{uri}/{segment.TrimStart('/').TrimEnd('/')}";
            }
            return uri;
        }

        public string AddQuery(string key, string value)
        {
            var encoded = Uri.EscapeDataString(value);
            var separator = url.Contains('?') ? "&" : "?";
            return $"{url}{separator}{key}={encoded}";
        }
    }


    extension(Uri uri)
    {
        public NameValueCollection GetQueryNameValueCollection()
        {
            return HttpUtility.ParseQueryString(uri.Query);
        }

        public IReadOnlyDictionary<string, string[]> GetQueryDictionary()
        {
            var collection = HttpUtility.ParseQueryString(uri.Query);
            var dict = new Dictionary<string, string[]>();

            foreach (var key in collection.AllKeys)
            {
                if (string.IsNullOrWhiteSpace(key)) continue;
                var values = collection.GetValues(key);

                dict[key] = values ?? [];
            }

            return dict.ToFrozenDictionary();
        }
    }
}