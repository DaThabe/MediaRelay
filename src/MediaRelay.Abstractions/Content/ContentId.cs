namespace MediaRelay.Content;

/// <summary>
/// 内容的唯一标识
/// </summary>
public readonly record struct ContentId : IEquatable<ContentId>
{
    public static ContentId Empty => default;

    private readonly string _value;
    private ContentId(string value) => _value = value;


    public static ContentId Create(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var trimmed = value.Trim();

        ArgumentException.ThrowIfNullOrWhiteSpace(trimmed);

        return new ContentId(trimmed);
    }
    public static ContentId Create(Guid guid)
    {
        if (guid == Guid.Empty) throw new ArgumentException("GUID cannot be empty.", nameof(guid));
        return new ContentId(guid.ToString("N"));
    }



    public bool Equals(ContentId? other) => _value.Equals(other?._value, StringComparison.OrdinalIgnoreCase);
    public override int GetHashCode() => _value.GetHashCode(StringComparison.OrdinalIgnoreCase);
    public override string ToString() => _value;
}
