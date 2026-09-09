namespace MediaRelay.Messaging;


public interface IMessage<out TContent>
{
    MessageId Id { get; }
    TContent Content { get; }
}


public readonly record struct MessageId : IEquatable<MessageId>
{
    public static MessageId Empty => default;

    private readonly string _value;
    private MessageId(string value) => _value = value;


    public static MessageId New()
    {
        return new MessageId(Guid.CreateVersion7().ToString("N"));
    }
    public static MessageId FromValue(string value)
    {
        var trimmed = value.Trim();

        if (string.IsNullOrWhiteSpace(trimmed))
            throw new ArgumentException("创建消息 Id 时不可使用空字符串", nameof(value));

        return new(trimmed);
    }


    public bool Equals(MessageId? other) => _value.Equals(other?._value, StringComparison.OrdinalIgnoreCase);
    public override int GetHashCode() => _value.GetHashCode(StringComparison.OrdinalIgnoreCase);
    public override string ToString() => _value;
}
