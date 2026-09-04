namespace MediaRelay.Storage;


public sealed record StorageOptions
{
    public DataStorageOptions Data { get; init; } = new();
    public CacheStorageOptions Cache { get; init; } = new();
}

public sealed record DataStorageOptions
{
    public string RootPath { get; init; } = "Datas";
    public bool EnableCompression { get; init; } = false;
    public bool EnableDeduplication { get; init; } = true;
    public int MaxFileSizeMB { get; init; } = 100;
}

public sealed record CacheStorageOptions
{
    public string RootPath { get; init; } = "Cache";
    public int ExpirationDays { get; init; } = 7;
    public long MaxCacheSizeMB { get; init; } = 1024;
    public string? EvictionPolicy { get; init; } = "LRU";
}