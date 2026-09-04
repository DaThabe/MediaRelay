namespace MediaRelay.Publish;


/// <summary>
/// 推送调度器
/// </summary>
public interface IPublishOrchestrator
{
    ValueTask PublishAsync(PublishContent content, CancellationToken cancellationToken = default);
}