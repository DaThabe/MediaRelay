using MediaRelay.Content.Extract;
using MediaRelay.Http;
using MediaRelay.Twitter.Image;
using MediaRelay.Twitter.Video;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace MediaRelay.Twitter.Tweet;


internal sealed class TweetContentExtractor(
        IServiceProvider serviceProvider,
        ImageUrl.Parser parser,
        ImageUrlResource.Factory factory,
        ITwitterVideoResourceFactory videoResourceFactory,
        IOptions<TwitterOptions> options,
        ILogger<TweetContentExtractor> logger
    ) : UrlSourceContentExtractor<TweetSource>(logger)
{
    protected override IServiceProvider ServiceProvider => serviceProvider;
    protected override IReadOnlySet<HttpCookieOptions> Cookies { get; } = options.Value.Http.Cookies.ToHashSet().AsReadOnly();
    protected override string ScriptFilePath { get; } = options.Value.Tweet.ExtractScriptPath;
    protected override UrlResourceParserHandler UrlResourceParser { get; } = url => factory.Create(parser.Parse(url));


    protected override async ValueTask ExtractAsync(BuildContext context, CancellationToken cancellationToken)
    {
        if (context.ContentSnapshot.Resources.Count != 0) return;


        var videoResource = await videoResourceFactory.CreateAsync(context.Source, cancellationToken);
        context.ContentBuilder.AddResources(videoResource);
    }
}