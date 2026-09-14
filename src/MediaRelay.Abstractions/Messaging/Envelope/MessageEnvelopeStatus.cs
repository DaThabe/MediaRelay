namespace MediaRelay.Messaging.Queue;


/// <summary>
/// 信封状态
/// </summary>
public enum MessageEnvelopeStatus
{
    /// <summary>
    /// 排队中
    /// </summary>
    Pending,

    /// <summary>
    /// 处理中
    /// </summary>
    Processing,

    /// <summary>
    /// 已完成
    /// </summary>
    Completed,

    /// <summary>
    /// 已拒绝
    /// </summary>
    Rejected,
}