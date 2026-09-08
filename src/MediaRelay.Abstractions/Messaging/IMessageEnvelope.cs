using System.Text.Json.Serialization;

namespace MediaRelay.Messaging;



public interface IMessageEnvelope<TMessage, out TContent>
    where TMessage : IMessage<TContent>
{
    TMessage Message { get; }
    MessageStatus Status { get; }
    DateTimeOffset CreateAt { get; }
}

public interface IMessage<out TContent>
{
    Guid Id { get; }
    TContent Content { get; }
}


[JsonConverter(typeof(JsonStringEnumConverter<MessageStatus>))]
public enum MessageStatus
{
    Pending,
    Failed,
    Dead
}