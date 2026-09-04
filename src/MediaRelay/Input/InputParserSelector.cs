using Microsoft.Extensions.Logging;

namespace MediaRelay.Input;


internal sealed partial class InputParserSelector(
        IEnumerable<IInputParser> parsers,
        ILogger<InputParserSelector> logger
    ) : IInputParserSelector
{
    private readonly IInputParser[] _parsers = [.. parsers];

    public IInputParser Select(string input)
    {
        foreach (var parser in _parsers)
        {
            if (!parser.CanParse(input)) continue;

            LogSelected(parser.GetType().Name, input);
            return parser;
        }

        throw new NotSupportedException($"无法解析的输入: {input}");
    }


    [LoggerMessage(Level = LogLevel.Debug, Message = "已选择 [{ParserType}] < 输入 [{Input}]")]
    private partial void LogSelected(string parserType, string input);
}