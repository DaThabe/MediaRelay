namespace MediaRelay.Resources;

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
        ArgumentNullException.ThrowIfNull(value);

        var trimmed = value.Trim();

        ArgumentException.ThrowIfNullOrWhiteSpace(trimmed);

        return new ResourceId(trimmed);
    }
    public static ResourceId Create(Guid guid)
    {
        if (guid == Guid.Empty) throw new ArgumentException("GUID cannot be empty.", nameof(guid));
        return new ResourceId(guid.ToString("N"));
    }



    public bool Equals(ResourceId? other) => _value.Equals(other?._value, StringComparison.OrdinalIgnoreCase);
    public override int GetHashCode() => _value.GetHashCode(StringComparison.OrdinalIgnoreCase);
    public override string ToString() => _value;
}
