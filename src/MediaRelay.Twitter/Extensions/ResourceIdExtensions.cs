#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
using MediaRelay.Twitter.Image;

namespace MediaRelay.Resources;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


internal static class ResourceIdExtensions
{
    extension(ResourceId)
    {
        public static ResourceId FromMediaIdAndImageSize(string mediaId, ImageSize size)
        {
            return ResourceId.Create($"Twiiter_Media:{mediaId}_{size}");
        }
    }
}
