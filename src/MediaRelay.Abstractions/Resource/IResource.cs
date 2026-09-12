using MediaRelay.Storage;

namespace MediaRelay.Resource;


/// <summary>
/// 用来获取媒体的二进制数据
/// </summary>
public interface IResource
{
    ResourceId Id { get; }
    MediaType Type { get; }


    ValueTask<Stream> GetStreamAsync(CancellationToken cancellationToken = default);
}