using Microsoft.Extensions.Logging;
using Spectre.Console;
using System.Collections.Frozen;
using System.Text;

namespace MediaRelay.Console.Logging;


internal sealed class ConsoleLogger(string categoryName, IAnsiConsole ansiConsole) : ILogger
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
        var messageMarkupString = MessagaeStyle.ToMarkupString(
            timestamp: DateTime.Now,
            level: logLevel,
            categoryName: categoryName,
            message: formatter(state, exception),
            exception: exception,
            scopeDatas: _currentScope?.ToFrozenDictionary());

        ansiConsole.MarkupLine(messageMarkupString);
    }
}


file sealed class MessagaeStyle
{
    private static readonly Style _timestampStyle = new(Color.Grey74, null, Decoration.Dim);

    private static readonly Style _levelTraceStyle = new(Color.Grey85, null, Decoration.Dim);
    private static readonly Style _levelDebugStyle = new(Color.DarkSlateGray2, null, Decoration.Bold);
    private static readonly Style _levelInformationStyle = new(Color.Gray69, null, null);
    private static readonly Style _levelWarningStyle = new(Color.SandyBrown, null, null);
    private static readonly Style _levelErrorStyle = new(Color.Red1, null, null);
    private static readonly Style _levelCriticalStyle = new(Color.Black, Color.Red1, null);
    private static readonly Style _levelNoneStyle = new(Color.Silver, null, null);

    private static readonly Style _categoryNameStyle = new(Color.Gray30, null, null);
    private static readonly Style _messageStyle = new(Color.White, null, Decoration.Bold);
    private static readonly Style _exceptionStyle = new(Color.Red, null, Decoration.Bold);

    private static readonly Style _scopeDataKeyStyle = new(Color.Gray30, null, Decoration.Bold);
    private static readonly Style _scopeDataValueStyle = new(Color.Gray30, null, Decoration.Bold);

    private static readonly Style _scopeDataParenthesesStyle = new(Color.Gray30, null, null);
    private static readonly Style _scopeDataEqualSignStyle = new(Color.Gray30, null, null);


    public static string ToMarkupString(
        DateTime timestamp,
        LogLevel level,
        string categoryName,
        string message,
        Exception? exception = null,
        IReadOnlyDictionary<string, object>? scopeDatas = null)
    {
        StringBuilder sb = new();

        var timestampMarkup = GetTimestampMarkup(timestamp);
        var levelMarkup = GetLevelMarkup(level);
        var categoryNameMarkup = GetCategoryNameMarkup(categoryName);
        var messageMarkup = GetMessageMarkup(message);
        var scopeDataMarkup = GetScopeDataMarkup(scopeDatas);
        var exceptionMarkup = GetExceptionMarkup(exception);

        // Title
        sb.AppendLine($"{timestampMarkup} {levelMarkup} {categoryNameMarkup}");

        // Message
        if (scopeDataMarkup is null)
            sb.Append($"    {messageMarkup}");
        else
            sb.Append($"    {messageMarkup} {scopeDataMarkup}");

        // Exception
        if (exceptionMarkup is not null)
        {
            sb.AppendLine();
            sb.Append($"    {exceptionMarkup}");
        }

        return sb.ToString();
    }

    private static string GetTimestampMarkup(DateTime timestamp)
    {
        return $"[{_timestampStyle.ToMarkup()}]{$"[{timestamp:HH:mm:ss}]".EscapeMarkup()}[/]";
    }
    private static string GetLevelMarkup(LogLevel level)
    {
        var (name, style) = level switch
        {
            LogLevel.Trace => ("TRC", _levelTraceStyle),
            LogLevel.Debug => ("DBG", _levelDebugStyle),
            LogLevel.Information => ("INF", _levelInformationStyle),
            LogLevel.Warning => ("WRN", _levelWarningStyle),
            LogLevel.Error => ("ERR", _levelErrorStyle),
            LogLevel.Critical => ("CRT", _levelCriticalStyle),
            _ => ("NON", _levelNoneStyle)
        };

        var title = $"[{name}]".EscapeMarkup();
        return $"[{style.ToMarkup()}]{title}[/]";
    }
    private static string? GetCategoryNameMarkup(string categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryName)) return null;
        return $"[{_categoryNameStyle.ToMarkup()}]{categoryName.EscapeMarkup()}[/]";
    }
    private static string? GetMessageMarkup(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return null;
        return $"[{_messageStyle.ToMarkup()}]{message.EscapeMarkup()}[/]";
    }
    private static string? GetExceptionMarkup(Exception? exception)
    {
        if (exception is null) return null;

        StringBuilder sb = new();
        sb.AppendLine($"{exception.GetType().Name}: {exception.Message}");

        var next = exception.InnerException;
        while (next is not null)
        {
            sb.AppendLine($"       →{next.GetType().Name}: {next.Message}");
            next = next.InnerException;
        }

        return $"[{_exceptionStyle.ToMarkup()}]{sb.ToString().EscapeMarkup()}[/]";
    }
    private static string? GetScopeDataMarkup(IReadOnlyDictionary<string, object>? datas)
    {
        if (datas is null || datas.Count == 0) return null;

        var items = new List<string>();

        foreach (var i in datas)
        {
            var item = $"[{_scopeDataKeyStyle.ToMarkup()}]{i.Key.EscapeMarkup()}[/][{_scopeDataEqualSignStyle.ToMarkup()}]=[/][{_scopeDataValueStyle.ToMarkup()}]{i.Value.ToString().EscapeMarkup()}[/] ";
            items.Add(item);
        }

        return $"[{_scopeDataParenthesesStyle.ToMarkup()}]{{[/] {string.Join(", ".EscapeMarkup(), items)} [{_scopeDataParenthesesStyle.ToMarkup()}]}}[/]";
    }
}
