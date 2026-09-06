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
        var trimmed = value.Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
            throw new ArgumentException("创建 SourceId 时不能使用空字符串", nameof(value));

        return new(trimmed);
    }


    public bool Equals(SourceId? other) => _value.Equals(other?._value, StringComparison.OrdinalIgnoreCase);
    public override int GetHashCode() => _value.GetHashCode(StringComparison.OrdinalIgnoreCase);
    public override string ToString() => _value;
}
