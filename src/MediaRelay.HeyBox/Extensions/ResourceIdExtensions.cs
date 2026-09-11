#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace MediaRelay.Resources;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


internal static class ResourceIdExtensions
{
    extension(ResourceId)
    {
        public static ResourceId FromPixivArtworkId(long artworkId)
        {
            return ResourceId.Create($"Pixiv_Original_Image:{artworkId}");
        }
    }
}
