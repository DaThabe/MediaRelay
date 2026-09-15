using MediaRelay.Browser;
using MediaRelay.Content.Extract;
using MediaRelay.Http;
using MediaRelay.Serializer;
using MediaRelay.Twitter.Image;
using MediaRelay.Twitter.Video;
using Microsoft.Extensions.Options;
namespace MediaRelay.Twitter.Tweet;


internal sealed class TweetContentExtractor(
        IPageSessionFactory pageSessionFactory,
        IJsonSerializerFactory jsonSerializerFactory,
        ImageUrl.Parser parser,
        ImageUrlResource.Factory factory,
        ITwitterVideoResourceFactory videoResourceFactory,
        IOptions<TwitterOptions> options
    ) : UrlSourceContentExtractor<TweetSource>
{
    protected override IPageSessionFactory PageSessionFactory { get; } = pageSessionFactory;
    protected override IJsonSerializerFactory JsonSerializerFactory { get; } = jsonSerializerFactory;
    protected override IReadOnlySet<HttpCookieOptions> Cookies { get; } = options.Value.Http.Cookies.ToHashSet().AsReadOnly();
    protected override string ScriptFilePath { get; } = options.Value.Tweet.ExtractScriptPath;
    protected override UrlResourceParserHandler UrlResourceParser { get; } = url => factory.Create(parser.Parse(url));


    protected override async ValueTask ExtractAsync(Context context, CancellationToken cancellationToken)
    {
        if (context.ContentSnapshot.Resources.Count != 0) return;


        var videoResource = await videoResourceFactory.CreateAsync(context.Source, cancellationToken);
        context.ContentBuilder.AddResources(videoResource);
    }
}