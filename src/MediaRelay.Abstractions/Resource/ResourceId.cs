namespace MediaRelay.Resource;

/// <summary>
/// 媒体资源的唯一标识
/// </summary>
public readonly record struct ResourceId : IEquatable<ResourceId>
{
    public static ResourceId Empty => default;

    private readonly string _value;
    private ResourceId(string value) => _value = value;


    public static ResourceId Create(string value)
    {
        var trimmed = value.Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
            throw new ArgumentException("创建资源 Id 时不可使用空字符串", nameof(value));

        return new ResourceId(trimmed);
    }


    public bool Equals(ResourceId? other) => _value.Equals(other?._value, StringComparison.OrdinalIgnoreCase);
    public override int GetHashCode() => _value.GetHashCode(StringComparison.OrdinalIgnoreCase);
    public override string ToString() => _value;
}
