#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace MediaRelay.Resource;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


internal static class ResourceIdExtensions
{
    extension(ResourceId)
    {
        public static ResourceId FromMediaHash(string mediaHash)
        {
            return ResourceId.Create($"HeyBox_Image:{mediaHash}");
        }
    }
}
