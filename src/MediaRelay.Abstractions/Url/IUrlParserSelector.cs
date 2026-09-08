namespace MediaRelay.Url;

public interface IUrlParserSelector
{
    IUrlParser Select(Uri uri);
}