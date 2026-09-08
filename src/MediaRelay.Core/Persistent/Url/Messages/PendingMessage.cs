namespace MediaRelay.Persistent.Url.Messages;

/// <summary>
/// 队列元素
/// </summary>
internal sealed record class PendingMessage : Message
{
    public static PendingMessage Create(Uri url)
    {
        return new PendingMessage() { Content = url };
    }

    public override bool TryNext(DateTimeOffset time, out Message? next)
    {
        next = ToFailed(time);
        return true;
    }

    public Message ToFailed(DateTimeOffset time, TimeSpan retryInterval, int maxRetries)
    {
        return new FailedMessage()
        {
            Content = Content,
            CreatedAt = time,
            MaxRetries = maxRetries,
            RetryInterval = retryInterval
        };
    }
    public Message ToFailed(DateTimeOffset time, TimeSpan retryInterval)
        => ToFailed(time, retryInterval, FailedMessage.DefaultMaxRetries);

    public Message ToFailed(DateTimeOffset time)
        => ToFailed(time, FailedMessage.DefaultRetryInterval, FailedMessage.DefaultMaxRetries);
}
