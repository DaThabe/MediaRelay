namespace MediaRelay.Persistent.Url.Messages;

internal sealed record class FailedMessage : Message
{
    public static int DefaultMaxRetries = 5;
    public static TimeSpan DefaultRetryInterval = TimeSpan.FromSeconds(5);


    public int RetryCount { get; init; }
    public int MaxRetries { get; init; } = DefaultMaxRetries;
    public TimeSpan RetryInterval { get; init; } = DefaultRetryInterval;


    public override bool TryNext(DateTimeOffset time, out Message? next)
    {
        var result = TryRetry(time, out var nextRetry);
        next = nextRetry;
        return result;
    }

    /// <summary>
    /// 根据输入的时间判断是否能重试
    /// </summary>
    public bool CanRetry(DateTimeOffset time)
    {
        return (time - CreatedAt) > RetryInterval && RetryCount < MaxRetries;
    }

    /// <summary>
    /// 用指定时间创建下一次重试  (如果不能重试则变成死信)
    /// </summary>
    public bool TryRetry(DateTimeOffset time, out FailedMessage? next)
    {
        if (CanRetry(time))
        {
            next = this with { RetryCount = RetryCount + 1 };
            return true;
        }

        next = null;
        return false;
    }
}
