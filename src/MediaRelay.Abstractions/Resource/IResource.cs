using MediaRelay.Storage;

namespace MediaRelay.Resource;


/// <summary>
/// 表示一个媒体文件数据
/// </summary>
public interface IResource
{
    ResourceId Id { get; }
    MediaType Type { get; }


    ValueTask<Stream> GetStreamAsync(CancellationToken cancellationToken = default);
}