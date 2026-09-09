using MediaRelay.Url;

namespace MediaRelay.Messaging.Queue;


internal sealed record class MessageEnvelope : IMessageEnvelope<UrlMessage, Uri>
{
    public required UrlMessage Message { get; init; }
    public MessageEnvelopeStatus Status { get; private set; } = MessageEnvelopeStatus.Pending;
    public DateTimeOffset CreateAt { get; init; } = DateTimeOffset.Now;
    public DateTimeOffset UpdatedAt { get; private set; } = DateTimeOffset.Now;
    public MessageRetryOptions? RetryOptions { get; init; }


    public static MessageEnvelope Create(UrlMessage message) =>
        new() { Message = message };
    public static MessageEnvelope Create(UrlMessage message, MessageRetryOptions retryOptions) =>
        new() { Message = message, RetryOptions = retryOptions };


    public void MarkCompleted()
    {
        if (Status == MessageEnvelopeStatus.Completed) return;

        if (Status is MessageEnvelopeStatus.Pending or MessageEnvelopeStatus.Processing)
        {
            OnUpdate(MessageEnvelopeStatus.Completed);
            return;
        }

        throw new InvalidOperationException($"当前状态不能完成: [{Status}]");
    }
    public void MarkProcessing()
    {
        if (Status == MessageEnvelopeStatus.Processing) return;

        if (Status is MessageEnvelopeStatus.Pending)
        {
            OnUpdate(MessageEnvelopeStatus.Processing);
            return;
        }

        throw new InvalidOperationException($"当前状态不能处理: [{Status}]");
    }
    public void MarkRejected()
    {
        if (Status == MessageEnvelopeStatus.Rejected) return;

        if (Status is MessageEnvelopeStatus.Processing or MessageEnvelopeStatus.Pending)
        {
            Status = MessageEnvelopeStatus.Rejected;
            return;
        }

        throw new InvalidOperationException($"当前状态不能拒绝: [{Status}]");
    }


    public void Recover()
    {
        OnUpdate(MessageEnvelopeStatus.Pending);
        RetryOptions?.Recover();
    }
    public void Retry()
    {
        if (RetryOptions is null)
        {
            throw new InvalidOperationException("该消息不能重试");
        }


        RetryOptions.Increment();
        OnUpdate(MessageEnvelopeStatus.Processing);
    }


    private void OnUpdate(MessageEnvelopeStatus status)
    {
        Status = status;
        UpdatedAt = DateTimeOffset.Now;
    }
}