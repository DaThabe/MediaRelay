using System.Text.Json.Serialization;

namespace MediaRelay.Persistent.Url.Messages;


[JsonDerivedType(typeof(PendingMessage), typeDiscriminator: "pending")]
[JsonDerivedType(typeof(FailedMessage), typeDiscriminator: "failed")]
[JsonDerivedType(typeof(DeadMessage), typeDiscriminator: "dead")]
public abstract record class Message
{
    public required Uri Value { get; init; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.Now;

    public virtual bool TryNext(DateTimeOffset time, out Message? next)
    {
        next = null;
        return false;
    }
}