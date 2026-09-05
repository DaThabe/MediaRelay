using Microsoft.Extensions.Logging;

namespace MediaRelay.Input;


internal sealed class InputParserSelector(
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

            using var _ = logger.Scope()
                .Add("InputParserName", parser.GetType().Name)
                .Begin();
            logger.LogDebug("已经选择输入解析器");

            return parser;
        }

        throw new NotSupportedException($"无法解析的输入: {input}");
    }
}