using MediaRelay.Browser;
using MediaRelay.Resource;
using MediaRelay.Storage;
using MediaRelay.Twitter.Tweet;
using Microsoft.Extensions.Options;

namespace MediaRelay.Twitter.Video;


internal interface ITwitterVideoResourceFactory
{
    ValueTask<IUrlResource> CreateAsync(TweetSource source, CancellationToken cancellationToken);
}

internal sealed class TwitterVideoResourceFactory(
        IBrowserService browserService,
        IUrlResourceFactory urlResourceFactory,
        IOptions<TwitterTweetOptions> options
    ) : ITwitterVideoResourceFactory
{
    public async ValueTask<IUrlResource> CreateAsync(TweetSource source, CancellationToken cancellationToken)
    {
        await using var context = await browserService.GetSharedContextAsync();
        await using var page = await context.NewPageAsync();
        await page.GotoAsync(options.Value.VideoDownloadUrl, cancellationToken: cancellationToken);

        var downloadUrl = await page.EvaluateScriptFileAsync<string>(options.Value.ExtractScriptPath, null, cancellationToken);

        var resourceId = ResourceId.Create(source.Id.ToString());
        return urlResourceFactory.Create(resourceId, new Uri(downloadUrl), MediaType.Mp4);
    }
}