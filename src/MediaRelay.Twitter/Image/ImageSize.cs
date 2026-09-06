namespace MediaRelay.Twitter.Image;

public sealed record class ImageSize
{
    public static ImageSize Thumbnail { get; } = new("small");
    public static ImageSize Original { get; } = new("4096x4096");


    /// <inheritdoc/>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    public static ImageSize FromName(string name)
    {
        var trimmed = name.ToLower().Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
            throw new ArgumentException("创建图像尺寸时名称不可为空字符串", nameof(name));

        if (trimmed == "small") return Thumbnail;
        if (trimmed == "4096x4096") return Original;

        throw new NotSupportedException($"不支持的图像尺寸: {trimmed}");
    }


    private readonly string _value;
    public ImageSize(string value) => _value = value;
    public override string ToString() => _value;
}