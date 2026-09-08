namespace MediaRelay.Storage;


public readonly record struct MediaType : IEquatable<MediaType>
{
    public static MediaType Empty => default;
    public static MediaType Jpg { get; } = new("jpg", TypeCategory.Image);
    public static MediaType Png { get; } = new("png", TypeCategory.Image);
    public static MediaType Mp4 { get; } = new("mp4", TypeCategory.Video);


    public static MediaType FromExtensions(string name)
    {
        var trimmed = name.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(trimmed)) return Empty;

        return trimmed switch
        {
            "jpg" or "jpeg" => Jpg,
            "png" => Png,
            "mp4" or "m4v" or "m4a" => Mp4,
            _ => new MediaType() { Extensions = trimmed, Category = TypeCategory.Unknown }
        };
    }


    public TypeCategory Category { get; init; }
    public string Extensions { get => field ?? string.Empty; init; }


    public bool IsImage => Category == TypeCategory.Image;
    public bool IsVideo => Category == TypeCategory.Video;


    private MediaType(string extension, TypeCategory category) =>
        (Extensions, Category) = (extension, category);
    public override string ToString() => Extensions;


    public bool Equals(MediaType other)
    {
        if (other.Category != Category) return false;
        return other.Extensions.Equals(Extensions, StringComparison.OrdinalIgnoreCase);
    }
    public override int GetHashCode()
    {
        return Extensions.GetHashCode(StringComparison.OrdinalIgnoreCase);
    }
}

public enum TypeCategory
{
    Unknown,
    Image,
    Video
}