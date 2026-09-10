namespace MediaRelay.Messaging.Queue;


public interface IMessageQueue<TMessage, TContent>
    where TMessage : IMessage<TContent>
{
    /// <summary>
    /// 入队
    /// </summary>
    ValueTask EnqueueAsync(TMessage message, CancellationToken cancellationToken = default);
    /// <summary>
    /// 出队最新的元素, 除非手动确认, 否则不会删除
    /// </summary>
    ValueTask<TMessage> DequeueAsync(CancellationToken cancellationToken = default);


    /// <summary>
    /// 确认
    /// </summary>
    ValueTask AcknowledgeAsync(MessageId messageId, CancellationToken cancellationToken = default);
    /// <summary>
    /// 拒绝
    /// </summary>
    ValueTask RejectAsync(MessageId messageId, CancellationToken cancellationToken = default);
}
