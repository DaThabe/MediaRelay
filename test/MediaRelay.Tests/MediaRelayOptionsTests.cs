using MediaRelay.Browser;
using MediaRelay.Clipboard;
using MediaRelay.HeyBox;
using MediaRelay.Http;
using MediaRelay.Immich;
using MediaRelay.Pixiv;
using MediaRelay.Storage;
using MediaRelay.Twitter;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace MediaRelay;


[TestClass]
public sealed class MediaRelayOptionsTests
{
    /// <summary>
    /// 直接读取随程序发布的 appsettings.json (由测试工程的 &lt;None Include /&gt; 链接到输出目录),
    /// 保证校验的是真实配置文件, 而不是测试里另抄的一份副本。
    /// 这里用 System.Text.Json 手工拍平成配置键, 以免为测试额外引入配置提供程序包。
    /// </summary>
    private static IConfiguration _configuration = null!;


    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        Assert.IsTrue(File.Exists(path), $"未找到 appsettings.json: {path}");

        var values = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        using var document = JsonDocument.Parse(File.ReadAllText(path));
        Flatten(document.RootElement, string.Empty, values);

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
    }


    /// <summary>
    /// 所有选项节点都必须位于 MediaRelay 根节点之下。
    /// 漏掉前缀 (例如写成 "Twitter:Http") 会让绑定静默失效, 并悄悄回退到代码默认值。
    /// </summary>
    [TestMethod]
    public void SectionPaths_Should_Be_Rooted_Under_MediaRelay()
    {
        string[] paths =
        [
            MediaRelayOptions.SectionPath,

            HttpOptions.SectionPath,
            BrowserOptions.SectionPath,
            BrowserLaunchOptions.SectionPath,
            BrowserNewContextOptions.SectionPath,
            StorageOptions.SectionPath,
            ClipboardOptions.SectionPath,
            ImmichOptions.SectionPath,

            UrlOptions.SectionPath,
            UrlMessageQueueOptions.SectionPath,

            PixivOptions.SectionPath,
            PixivHttpOptions.SectionPath,
            PixivArtworkOptions.SectionPath,
            PixivArtworkUrlOptions.SectionPath,
            PixivOriginalImageUrlOptions.SectionPath,

            TwitterOptions.SectionPath,
            TwitterHttpOptions.SectionPath,
            TwitterImageUrlOptions.SectionPath,
            TwitterTweetOptions.SectionPath,
            TwitterTweetUrlOptions.SectionPath,

            HeyBoxOptions.SectionPath,
            HeyBoxHttpOptions.SectionPath,
            HeyBoxBbsLinkOptions.SectionPath,
            HeyBoxBbsLinkUrlOptions.SectionPath,
            HeyBoxOriginalImageUrlOptions.SectionPath,
        ];

        var root = MediaRelayOptions.SectionPath;

        foreach (var path in paths)
        {
            var isRooted = path == root
                || path.StartsWith($"{root}:", StringComparison.Ordinal);

            Assert.IsTrue(isRooted, $"选项节点必须位于 '{root}' 之下, 实际为 '{path}'");
        }
    }


    /// <summary>
    /// appsettings.json 里出现的键必须真正写入选项对象。
    /// 断言把「配置里的值」与「绑定后的属性值」直接比对, 键名不一致时会立即失败。
    /// </summary>
    [TestMethod]
    public void UrlMessageQueueOptions_AppSettingsKeys_Should_Bind()
    {
        var sectionPath = UrlMessageQueueOptions.SectionPath;

        var bound = new UrlMessageQueueOptions();
        _configuration.GetSection(sectionPath).Bind(bound);

        Assert.AreEqual(_configuration[$"{sectionPath}:FilePath"], bound.FilePath);
        Assert.AreEqual("UrlMessages.json", bound.FilePath);

        Assert.AreEqual(_configuration[$"{sectionPath}:RetryCount"], bound.RetryCount.ToString());
        Assert.AreEqual(5, bound.RetryCount);
    }

    [TestMethod]
    public void HeyBoxBbsLinkUrlOptions_AppSettingsKeys_Should_Bind()
    {
        var sectionPath = HeyBoxBbsLinkUrlOptions.SectionPath;

        var bound = new HeyBoxBbsLinkUrlOptions();
        _configuration.GetSection(sectionPath).Bind(bound);

        Assert.AreEqual(_configuration[$"{sectionPath}:Format"], bound.Format);
        Assert.AreEqual(_configuration[$"{sectionPath}:LinkIdKey"], bound.LinkIdKey);
        Assert.AreEqual("id", bound.LinkIdKey);
    }


    /// <summary>
    /// 把嵌套 JSON 拍平成 "A:B:C" 形式的配置键, 数组按索引展开 (例如 Cookies:0:Name)。
    /// </summary>
    private static void Flatten(JsonElement element, string prefix, Dictionary<string, string?> target)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
            {
                foreach (var property in element.EnumerateObject())
                {
                    var key = string.IsNullOrEmpty(prefix)
                        ? property.Name
                        : $"{prefix}:{property.Name}";

                    Flatten(property.Value, key, target);
                }

                break;
            }

            case JsonValueKind.Array:
            {
                var index = 0;

                foreach (var item in element.EnumerateArray())
                {
                    Flatten(item, $"{prefix}:{index}", target);
                    index++;
                }

                break;
            }

            case JsonValueKind.Null:
            {
                target[prefix] = null;
                break;
            }

            default:
            {
                target[prefix] = element.ToString();
                break;
            }
        }
    }
}
