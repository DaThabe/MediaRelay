namespace MediaRelay.Persistent;


public sealed record class PersistentOptions
{
    public static string Name { get; set; } = "Persistent";
    public string UrlMessagesFile { get; set; } = "UrlMessages.json";
}