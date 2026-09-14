namespace MediaRelay.Storage;


public sealed record class StorageFileName : IEquatable<StorageFileName>
{
    private readonly string _value;
    private StorageFileName(string value) => _value = value;


    public override string ToString() =>
        _value;

    public bool Equals(StorageFileName? other) =>
        (other?._value ?? string.Empty).Equals(_value ?? string.Empty, StringComparison.OrdinalIgnoreCase);

    public override int GetHashCode() =>
        _value.GetHashCode(StringComparison.OrdinalIgnoreCase);



    public static StorageFileName Create(string raw)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(raw);

        raw = raw.Trim();

        // 1. 不能含路径分隔符
        if (raw.Contains('/') || raw.Contains('\\'))
            throw new ArgumentException("文件名不能包含路径分隔符", nameof(raw));

        // 2. 不能含非法字符
        if (raw.IndexOfAny(InvalidChars) >= 0)
            throw new ArgumentException("文件名包含非法字符", nameof(raw));

        // 3. 不能是 . 或 ..
        if (raw is "." or "..")
            throw new ArgumentException("文件名不能是 . 或 ..", nameof(raw));

        // 4. 不能以空格或点结尾
        if (raw.EndsWith(' ') || raw.EndsWith('.'))
            throw new ArgumentException("文件名不能以空格或点结尾", nameof(raw));

        // 5. 长度限制
        if (raw.Length > 255)
            throw new ArgumentException("文件名过长", nameof(raw));

        // 6. Windows 保留名（带扩展名也算）
        var nameWithoutExt = Path.GetFileNameWithoutExtension(raw);
        if (ReservedNames.Contains(nameWithoutExt))
            throw new ArgumentException("文件名为系统保留名", nameof(raw));

        return new StorageFileName(raw);
    }


    private static readonly HashSet<string> ReservedNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "CON", "PRN", "AUX", "NUL",
        "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
        "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
    };

    private static readonly char[] InvalidChars =
        Path.GetInvalidFileNameChars().Concat(['\0']).Distinct().ToArray();
}