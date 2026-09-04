using MediaRelay.Source;

namespace MediaRelay.Input;

/// <summary>
/// 外部输入的来源解析器, 解析为可使用的来源信息
/// </summary>
public interface IInputParser
{
    bool CanParse(string input);
    ISource Parse(string input);
}
