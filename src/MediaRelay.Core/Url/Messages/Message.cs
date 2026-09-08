using MediaRelay.Url;
using System.Text.Json.Serialization;

namespace MediaRelay.Url.Messages;


[JsonDerivedType(typeof(PendingMessage), typeDiscriminator: "pending")]
[JsonDerivedType(typeof(FailedMessage), typeDiscriminator: "failed")]
[JsonDerivedType(typeof(DeadMessage), typeDiscriminator: "dead")]
public abstract record class Message : IUrlMessage
{
    public required Uri Content { get; init; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.Now;

    public virtual bool TryNext(DateTimeOffset time, out Message? next)
    {
        next = null;
        return false;
    }

    bool IUrlMessage.TryNext(DateTimeOffset time, out IUrlMessage? next)
    {
        var result = TryNext(time, out var value);
        next = value;
        return result;
    }
}
