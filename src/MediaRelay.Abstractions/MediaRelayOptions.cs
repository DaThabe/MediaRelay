using MediaRelay.Browser;
using MediaRelay.Http;
using MediaRelay.Storage;

namespace MediaRelay;


public sealed record class MediaRelayOptions
{
    public const string SectionName = "MediaRelay";
    public const string SectionPath = SectionName;

    [Obsolete("请使用  MediaRelay.Url.UrlOptions")]
    public string UrlMessageQueueFile { get; set; } = "UrlMessages.json";
    public HttpOptions Http { get; set; } = new();
    public BrowserOptions Browser { get; set; } = new();
    public StorageOptions Storage { get; set; } = new();
}
