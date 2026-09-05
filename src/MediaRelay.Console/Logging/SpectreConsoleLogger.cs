using Microsoft.Extensions.Logging;
using Spectre.Console;
using System.Collections.Frozen;
using System.Text;

namespace MediaRelay.Console.Logging;

internal sealed partial class SpectreConsoleLogger(string categoryName) : ILogger
{
    private LoggerScope? _rootScope;
    private LoggerScope? _currentScope;

    public bool IsEnabled(LogLevel logLevel) => true;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        if (state is not IDictionary<string, object> dict) return null;

        if (_rootScope is null)
        {
            _rootScope = new();
            _rootScope.AddRange(dict);
            _currentScope = _rootScope;
        }
        else
        {
            var childScope = _rootScope.CreateChildScope();
            childScope.AddRange(dict);
            _currentScope = childScope;
        }

        return _currentScope;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var messageMarkup = ToMarkup(
            timestamp: DateTime.Now,
            level: logLevel,
            categoryName: categoryName,
            message: formatter(state, exception),
            scopeDatas: _currentScope?.ToFrozenDictionary() ?? FrozenDictionary<string, object>.Empty);

        AnsiConsole.MarkupLine(messageMarkup);
        if (exception is not null) AnsiConsole.WriteException(exception, ExceptionFormats.NoStackTrace);
    }
}

internal sealed partial class SpectreConsoleLogger
{
    private readonly Style _timestampStyle = new(Color.Grey74, null, Decoration.Dim);

    private readonly Style _levelTraceStyle = new(Color.Grey85, null, Decoration.Dim);
    private readonly Style _levelDebugStyle = new(Color.DarkSlateGray2, null, Decoration.Bold);
    private readonly Style _levelInformationStyle = new(Color.Gray69, null, null);
    private readonly Style _levelWarningStyle = new(Color.SandyBrown, null, null);
    private readonly Style _levelErrorStyle = new(Color.Red1, null, null);
    private readonly Style _levelCriticalStyle = new(Color.Black, Color.Red1, null);
    private readonly Style _levelNoneStyle = new(Color.Silver, null, null);

    private readonly Style _categoryNameStyle = new(Color.Gray30, null, null);
    private readonly Style _messageStyle = new(Color.White, null, Decoration.Bold);

    private readonly Style _scopeDataKeyStyle = new(Color.Gray30, null, Decoration.Bold);
    private readonly Style _scopeDataValueStyle = new(Color.Gray30, null, Decoration.Bold);

    private readonly Style _scopeDataParenthesesStyle = new(Color.Gray30, null, null);
    private readonly Style _scopeDataEqualSignStyle = new(Color.Gray30, null, null);


    private string ToMarkup(DateTime timestamp, LogLevel level, string categoryName, string message, IReadOnlyDictionary<string, object> scopeDatas)
    {
        StringBuilder sb = new();

        // Time
        var timestampMarkup = $"[{_timestampStyle.ToMarkup()}]{$"[{timestamp:HH:mm:ss}]".EscapeMarkup()}[/]";
        // Level
        var (levelName, levelStyle) = GetLevelStyle(level);
        var levelMarkup = $"[{levelStyle.ToMarkup()}]{$"[{levelName}]".EscapeMarkup()}[/]";
        // Category
        var categoryNameMarkup = $"[{_categoryNameStyle.ToMarkup()}]{categoryName.EscapeMarkup()}[/]";
        // Message
        var messageMarkup = $"[{_messageStyle.ToMarkup()}]{message.EscapeMarkup()}[/] {GetScopeDataMarkup(scopeDatas)}";

        // Format
        sb.AppendLine($"{timestampMarkup} {levelMarkup} {categoryNameMarkup}");
        sb.Append($"    {messageMarkup}");

        return sb.ToString();
    }


    private (string Name, Style Style) GetLevelStyle(LogLevel level) => level switch
    {
        LogLevel.Trace => ("TRC", _levelTraceStyle),
        LogLevel.Debug => ("DBG", _levelDebugStyle),
        LogLevel.Information => ("INF", _levelInformationStyle),
        LogLevel.Warning => ("WRN", _levelWarningStyle),
        LogLevel.Error => ("ERR", _levelErrorStyle),
        LogLevel.Critical => ("CRT", _levelCriticalStyle),
        _ => ("NON", _levelNoneStyle)
    };

    private string GetScopeDataMarkup(IReadOnlyDictionary<string, object> datas)
    {
        if (datas.Count <= 0) return string.Empty;

        var items = new List<string>();

        foreach (var i in datas)
        {
            var item = $"[{_scopeDataKeyStyle.ToMarkup()}]{i.Key.EscapeMarkup()}[/] [{_scopeDataEqualSignStyle.ToMarkup()}]=[/] [{_scopeDataValueStyle.ToMarkup()}]{i.Value.ToString().EscapeMarkup()}[/] ";
            items.Add(item);
        }

        return $"[{_scopeDataParenthesesStyle.ToMarkup()}]{{[/] {string.Join(", ".EscapeMarkup(), items)} [{_scopeDataParenthesesStyle.ToMarkup()}]}}[/]";
    }
}
