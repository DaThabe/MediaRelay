using MediaRelay.Source;

namespace MediaRelay;


/// <summary>
/// 枚举转发
/// </summary>
public interface IMediaRelay
{
    /// <summary>
    /// 将内容转发到指定的目标, 并返回一个转发任务的唯一标识
    /// </summary>
    ValueTask HandleAsync(ISource source, CancellationToken cancellationToken = default);
}