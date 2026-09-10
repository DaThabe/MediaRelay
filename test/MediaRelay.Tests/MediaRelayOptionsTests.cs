using MediaRelay.Browser;
using MediaRelay.Http;
using MediaRelay.Immich;
using MediaRelay.Pixiv;
using MediaRelay.Storage;
using MediaRelay.Twitter;

namespace MediaRelay;


[TestClass]
public sealed class MediaRelayOptionsTests
{
    [TestMethod]
    [DataRow(MediaRelayOptions.SectionPath, DisplayName = $"{MediaRelayOptions.SectionName}={MediaRelayOptions.SectionPath}")]
    // Http
    [DataRow(HttpOptions.SectionPath, DisplayName = $"{HttpOptions.SectionName}={HttpOptions.SectionPath}")]
    // Browser
    [DataRow(BrowserOptions.SectionPath, DisplayName = $"{BrowserOptions.SectionName}={BrowserOptions.SectionPath}")]
    [DataRow(BrowserLaunchOptions.SectionPath, DisplayName = $"{BrowserLaunchOptions.SectionName}={BrowserLaunchOptions.SectionPath}")]
    // Storage
    [DataRow(StorageOptions.SectionPath, DisplayName = $"{StorageOptions.SectionName}={StorageOptions.SectionPath}")]
    // Immich
    [DataRow(ImmichOptions.SectionPath, DisplayName = $"{ImmichOptions.SectionName}={ImmichOptions.SectionPath}")]
    // Twitter
    [DataRow(TwitterOptions.SectionPath, DisplayName = $"{TwitterOptions.SectionName}={TwitterOptions.SectionPath}")]
    [DataRow(TwitterHttpOptions.SectionPath, DisplayName = $"{TwitterHttpOptions.SectionName}={TwitterHttpOptions.SectionPath}")]
    [DataRow(TwitterTweetOptions.SectionPath, DisplayName = $"{TwitterTweetOptions.SectionName}={TwitterTweetOptions.SectionPath}")]
    [DataRow(TwitterTweetUrlOptions.SectionPath, DisplayName = $"{TwitterTweetUrlOptions.SectionName}={TwitterTweetUrlOptions.SectionPath}")]
    [DataRow(TwitterImageUrlOptions.SectionPath, DisplayName = $"{TwitterImageUrlOptions.SectionName}={TwitterImageUrlOptions.SectionPath}")]
    // Pixiv
    [DataRow(PixivOptions.SectionPath, DisplayName = $"{PixivOptions.SectionName}={PixivOptions.SectionPath}")]
    [DataRow(PixivHttpOptions.SectionPath, DisplayName = $"{PixivHttpOptions.SectionName}={PixivHttpOptions.SectionPath}")]
    [DataRow(PixivArtworkOptions.SectionPath, DisplayName = $"{PixivArtworkOptions.SectionName}={PixivArtworkOptions.SectionPath}")]
    [DataRow(PixivArtworkUrlOptions.SectionPath, DisplayName = $"{PixivArtworkUrlOptions.SectionName}={PixivArtworkUrlOptions.SectionPath}")]
    [DataRow(PixivOriginalImageUrlOptions.SectionPath, DisplayName = $"{PixivOriginalImageUrlOptions.SectionName}={PixivOriginalImageUrlOptions.SectionPath}")]
    public void Show_Selection_Key_Path(string path)
    {
        System.Console.Write(path);
    }
}
