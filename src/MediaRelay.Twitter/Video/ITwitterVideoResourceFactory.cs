using MediaRelay.Browser;
using MediaRelay.Http;
using MediaRelay.Resource;
using MediaRelay.Storage.Media;
using MediaRelay.Twitter.Tweet;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace MediaRelay.Twitter.Video;


internal interface ITwitterVideoResourceFactory
{
    ValueTask<IUrlResource> CreateAsync(TweetSource source, CancellationToken cancellationToken);
}

internal sealed partial class TwitterVideoResourceFactory(
        IHttpClient httpClient,
        IPageSessionFactory pageSessionFactory,
        IUrlResourceFactory urlResourceFactory,
        IOptions<TwitterTweetOptions> options
    ) : ITwitterVideoResourceFactory
{
    public async ValueTask<IUrlResource> CreateAsync(TweetSource source, CancellationToken cancellationToken)
    {
        var formData = new Dictionary<string, string>
        {
            ["q"] = source.Url.ToString(),
            ["lang"] = "en",
            ["cftoken"] = "",
        };

        HttpRequestMessage req = new(HttpMethod.Post, "https://savetwitter.net/api/ajaxSearch")
        {
            Content = new FormUrlEncodedContent(formData)
        };

        var resp = await httpClient.SendAsync(req, cancellationToken);
        var respString = await resp.Content.ReadAsStringAsync(cancellationToken);

        var matchResult = Regex().Match(respString);
        if (!matchResult.Success) throw new InvalidOperationException("无法获取下载连接");

        var downloadUrl = Regex().Match(respString).Groups["url"].Value;
        var resourceId = ResourceId.Create(source.Id.ToString());
        return urlResourceFactory.Create(resourceId, new Uri(downloadUrl), MediaType.Mp4);


        //await using var pageSession = await pageSessionFactory.CreateAsync();
        //await pageSession.GotoAsync(options.Value.VideoDownloadUrl, cancellationToken: cancellationToken);

        //var tweetUrl = source.Url.ToString();
        //var downloadUrl = await pageSession.EvaluateScriptFileAsync<string>(options.Value.ExtractScriptPath, tweetUrl, cancellationToken);

        //var resourceId = ResourceId.Create(source.Id.ToString());
        //return urlResourceFactory.Create(resourceId, new Uri(downloadUrl), MediaType.Mp4);
    }


    [GeneratedRegex(@"href=\\""(?<url>.+?)\\""", RegexOptions.Compiled)]
    private static partial Regex Regex();
}