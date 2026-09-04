namespace MediaRelay.Input;


/// <summary>
/// 根据输入选择合适的来源解析器
/// </summary>
public interface IInputParserSelector
{
    IInputParser Select(string input);
}
