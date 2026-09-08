using MediaRelay.Source;

namespace MediaRelay.Url;


/// <summary>
/// 外部输入的来源解析器, 解析为可使用的来源信息
/// </summary>
public interface IUrlParser
{
    bool CanParse(Uri url);
    IUrlSource Parse(Uri url);
}
