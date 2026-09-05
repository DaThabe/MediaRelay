namespace MediaRelay.Storage;


public sealed record StorageOptions
{
    public static string Name { get; set; } = "Storage";


    public string RootPath { get; set; } = "./Storage/";
}