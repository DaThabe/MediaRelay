#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
using MediaRelay.Twitter.Tweet;

namespace MediaRelay.Source;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


internal static class SourceIdExtensions
{
    extension(SourceId)
    {
        public static SourceId FromUsernameAndTweetId(string username, string tweetId)
        {
            return SourceId.Create($"Twitter_Tweet:{username}_{tweetId}");
        }
    }
}
