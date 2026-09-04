namespace MediaRelay.Publish;


/// <summary>
/// 推送执行器
/// </summary>
public interface IPublishExecutor
{
    ValueTask<PublishJobId> ExecuteAsync(PublishContent content, CancellationToken cancellationToken = default);
}