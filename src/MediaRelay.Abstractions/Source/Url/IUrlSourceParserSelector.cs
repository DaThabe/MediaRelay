namespace MediaRelay.Source.Url;

public interface IUrlSourceParserSelector
{
    IUrlSourceParser Select(Uri uri);
}