namespace MediaRelay.Source;

[Obsolete]
public interface IUrlParserSelector
{
    IUrlSourceParser Select(Uri uri);
}