namespace MediaRelay;


/// <summary>
/// 转发内容
/// </summary>
public interface IRelay
{
    ValueTask RelayAsync(RelayContent content, CancellationToken cancellationToken = default);
}