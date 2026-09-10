using MediaRelay.Messaging.Queue;

namespace MediaRelay.Url.Messaging;


internal sealed record class UrlMessageEnvelope : IMessageEnvelope<UrlMessage, Uri>
{
    private MessageEnvelopeStatus _status = MessageEnvelopeStatus.Pending;

#pragma warning disable RCS1085 // Use auto-implemented property  Json反序列化无法设置 private set;
    public MessageEnvelopeStatus Status { get => _status; init => _status = value; }
#pragma warning restore RCS1085 // Use auto-implemented property


    public required UrlMessage Message { get; init; }
    public DateTimeOffset CreateAt { get; init; } = DateTimeOffset.Now;
    public DateTimeOffset UpdatedAt { get; private set; } = DateTimeOffset.Now;
    public UrlMessageRetryCounter? RetryCounter { get; init; }


    public static UrlMessageEnvelope Create(UrlMessage message) =>
        new() { Message = message };
    public static UrlMessageEnvelope Create(UrlMessage message, UrlMessageRetryCounter retryOptions) =>
        new() { Message = message, RetryCounter = retryOptions };


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
            OnUpdate(MessageEnvelopeStatus.Rejected);
            return;
        }

        throw new InvalidOperationException($"当前状态不能拒绝: [{Status}]");
    }


    public void Recover()
    {
        OnUpdate(MessageEnvelopeStatus.Pending);
        RetryCounter?.Recover();
    }
    public bool TryRetry()
    {
        if (RetryCounter is null) return false;

        if (RetryCounter.TryIncrement())
        {
            OnUpdate(MessageEnvelopeStatus.Processing);
            return true;
        }

        return false;
    }


    private void OnUpdate(MessageEnvelopeStatus status)
    {
        _status = status;
        UpdatedAt = DateTimeOffset.Now;
    }
}