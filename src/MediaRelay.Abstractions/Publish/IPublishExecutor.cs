namespace MediaRelay.Publish;


/// <summary>
/// 推送执行器
/// </summary>
[Obsolete]
public interface IPublishExecutor
{
    ValueTask ExecuteAsync(RelayContent content, CancellationToken cancellationToken = default);
}