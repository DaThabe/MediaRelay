namespace MediaRelay.Source.Url;


/// <summary>
/// 网址来源解析器
/// </summary>
public interface IUrlSourceParser
{
    bool CanParse(Uri url);
    IUrlSource Parse(Uri url);
}