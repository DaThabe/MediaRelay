using System.Text.Json.Serialization;

namespace MediaRelay.Messaging.Queue;


/// <summary>
/// 信封状态
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<MessageEnvelopeStatus>))]
public enum MessageEnvelopeStatus
{
    /// <summary>
    /// 排队种
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
    /// 失败
    /// </summary>
    Failed,
}