using System.Text.Json.Serialization;

namespace MediaRelay.Messaging;



public interface IMessageEnvelope<TMessage, out TContent>
    where TMessage : IMessage<TContent>
{
    TMessage Message { get; }
    MessageStatus Status { get; }
    DateTimeOffset CreateAt { get; }
}


[JsonConverter(typeof(JsonStringEnumConverter<MessageStatus>))]
public enum MessageStatus
{
    Pending,
    Failed,
    Dead
}