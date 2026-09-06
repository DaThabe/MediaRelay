#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace MediaRelay.Source;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


internal static class SourceIdExtensions
{
    extension(SourceId)
    {
        public static SourceId FromPixivArtworkId(long artworkId)
        {
            return SourceId.Create($"Pixiv_Artwork:{artworkId}");
        }
    }
}
