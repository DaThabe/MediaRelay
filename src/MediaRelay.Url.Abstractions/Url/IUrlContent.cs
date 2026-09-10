using MediaRelay.Content;
using MediaRelay.Source;

namespace MediaRelay.Url;


public interface IUrlContent : IContent
{
    ISource IContent.Source => Source;
    new IUrlSource Source { get; }
}