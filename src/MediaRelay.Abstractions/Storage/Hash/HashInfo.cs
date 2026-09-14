using System.Text.Json.Serialization;

namespace MediaRelay.Storage.Hash;

public sealed record class HashInfo : IEquatable<HashInfo>
{
    public const string SHA256 = "SHA256";
    public const string SHA1 = "SHA1";
    public const string MD5 = "MD5";



    public string Algorithm { get; init; }
    public byte[] Data { get; init; }


    [JsonIgnore] public int Length => Data.Length;
    [JsonIgnore] public string HexString => Convert.ToHexString(Data).ToLowerInvariant();
    [JsonIgnore] public string Base64String => Convert.ToBase64String(Data);


    [JsonConstructor]
    private HashInfo(string algorithm, byte[] data) => (Algorithm, Data) = (algorithm, data);
    public static HashInfo Create(string algorithm, ReadOnlySpan<byte> data)
    {
        if (string.IsNullOrWhiteSpace(algorithm))
            throw new ArgumentException("Hash算法类型不可为空", nameof(algorithm));

        if (data.Length == 0)
            throw new ArgumentException("Hash数据长度必须大于0");

        return new(algorithm.Trim().ToUpperInvariant(), data.ToArray());
    }
    public static HashInfo FromSHA256(ReadOnlySpan<byte> data) =>
        Create(SHA256, data);



    public bool Equals(HashInfo? other)
    {
        if (other is null) return false;
        if (!other.Algorithm.Equals(Algorithm, StringComparison.OrdinalIgnoreCase)) return false;
        if (other.Data.Length != Data.Length) return false;

        return other.Data.SequenceEqual(Data);
    }
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Algorithm, StringComparer.OrdinalIgnoreCase);

        foreach (var b in Data) hashCode.Add(b);

        return hashCode.ToHashCode();
    }
    public override string ToString()
    {
        return $"{Algorithm}:{HexString}";
    }
}