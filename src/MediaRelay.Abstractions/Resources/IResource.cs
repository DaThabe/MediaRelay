namespace MediaRelay.Resources;


/// <summary>
/// 用来获取媒体的二进制数据
/// </summary>
public interface IResource
{
    ResourceId Id { get; }
    string Extensions { get; }


    ValueTask<Stream> GetStreamAsync(CancellationToken cancellationToken = default);
}