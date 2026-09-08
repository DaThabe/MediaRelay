namespace MediaRelay.Persistent.Url.Messages;

internal sealed record class DeadMessage : Message
{
    public string? Reason { get; init; }
    public required int RetryCount { get; init; }


    /// <summary>
    /// 创建一个已到最大尝试次数的死信
    /// </summary>
    public static DeadMessage CreateMaxRetries(Uri value, int count, DateTimeOffset time, string reason = "已到最大尝试次数")
    {
        return new()
        {
            Content = value,
            CreatedAt = time,
            RetryCount = count,
            Reason = reason
        };
    }
}