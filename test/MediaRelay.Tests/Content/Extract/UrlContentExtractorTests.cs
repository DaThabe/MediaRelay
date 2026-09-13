using MediaRelay.Browser;
using MediaRelay.Http;
using MediaRelay.Resource;
using MediaRelay.Source;

namespace MediaRelay.Content.Extract;

[TestClass]
public sealed class UrlContentExtractorTests
{
    [TestMethod]
    public void Fuck()
    {

    }
}


file sealed class TestUrlContentExtractor(IBrowserService browserService) : DefaultUrlContentExtractor(browserService)
{
    protected override IEnumerable<HttpCookieOptions> GetCookies()
    {
        throw new NotImplementedException();
    }

    protected override string GetScriptFilePath()
    {
        throw new NotImplementedException();
    }

    protected override IResource ToResource(string resourceUrl)
    {
        throw new NotImplementedException();
    }
}