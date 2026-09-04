namespace MediaRelay.Resources;

/// <summary>
/// 媒体类型 包含类型, 格式, 扩展名等
/// </summary>
public sealed record class MediaType : IEquatable<MediaType>
{
    /// <summary>
    /// Image/Png
    /// </summary>
    public static MediaType Png { get; } = new()
    {
        Type = "Image",
        Format = "Png",
        Extension = ".png"
    };

    /// <summary>
    /// Image/Jpeg
    /// </summary>
    public static MediaType Jpeg { get; } = new()
    {
        Type = "Image",
        Format = "Jpeg",
        Extension = ".jpg"
    };


    /// <summary>
    /// 媒体类型 (如 Image/Video/等等)
    /// </summary>
    public required string Type { get; init; }

    /// <summary>
    /// 媒体格式 (如 mp4, jpg )
    /// </summary>
    public required string Format { get; init; }

    /// <summary>
    /// 文件扩展名 (如 .mp4, .jpg )
    /// </summary>
    public required string Extension { get; init; }


    /// <summary>
    /// 根据类型和格式判断是否相等
    /// </summary>
    public bool Equals(MediaType? other)
    {
        if (other is null) return false;

        return Type.Equals(other.Type, StringComparison.OrdinalIgnoreCase)
            && Format.Equals(other.Format, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine
        (
            Type.GetHashCode(StringComparison.OrdinalIgnoreCase),
            Format.GetHashCode(StringComparison.OrdinalIgnoreCase)
        );
    }

    public override string ToString()
    {
        return $"{Type}/{Format}";
    }
}