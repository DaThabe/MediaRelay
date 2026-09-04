namespace MediaRelay.Source;

/// <summary>
/// 内容的唯一标识
/// </summary>
public readonly record struct SourceId : IEquatable<SourceId>
{
    public static SourceId Empty => default;

    private readonly string _value;
    private SourceId(string value) => _value = value;


    public static SourceId Create(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var trimmed = value.Trim();

        ArgumentException.ThrowIfNullOrWhiteSpace(trimmed);

        return new(trimmed);
    }
    public static SourceId Create(Guid guid)
    {
        if (guid == Guid.Empty) throw new ArgumentException("GUID cannot be empty.", nameof(guid));
        return new(guid.ToString("N"));
    }



    public bool Equals(SourceId? other) => _value.Equals(other?._value, StringComparison.OrdinalIgnoreCase);
    public override int GetHashCode() => _value.GetHashCode(StringComparison.OrdinalIgnoreCase);
    public override string ToString() => _value;
}
