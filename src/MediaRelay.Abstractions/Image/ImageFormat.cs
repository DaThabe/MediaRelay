namespace MediaRelay.Image;

public sealed record class ImageFormat
{
    public static ImageFormat Jpg { get; } = new("jpg");
    public static ImageFormat Png { get; } = new("png");


    public static ImageFormat FromName(string name)
    {
        var trimmed = name.ToLower().Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
            throw new ArgumentException("创建图像格式不可使用空字符串", nameof(name));

        if (trimmed == "jpg") return Jpg;
        if (trimmed == "png") return Png;

        throw new NotSupportedException($"不支持的图像格式: {trimmed}");
    }


    private readonly string _value;
    public ImageFormat(string value) => _value = value;
    public override string ToString() => _value;
}