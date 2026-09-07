using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Json;
using Spectre.Console.Rendering;
using System.Diagnostics;
using System.Text.Json;

namespace MediaRelay.Console.Logging.Json;

internal sealed partial class JsonConsoleLogger(string categoryName) : ILogger
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
        var title = GetTitleMarkupString(logLevel, categoryName, formatter(state, exception));
        var scopeData = GetScopeDataJsonText(_currentScope?.ToDictionary());


        if (scopeData is null)
        {
            AnsiConsole.MarkupLine(title);
            return;
        }
        if (exception is null)
        {
            AnsiConsole.Write(new Rows(new Markup(title), scopeData));
            return;
        }

        AnsiConsole.WriteException(exception, ExceptionFormats.NoStackTrace);
    }

    private static string GetTitleMarkupString(LogLevel level, string categoryName, string message)
    {
        var levelName = level switch
        {
            LogLevel.Trace => "TRC",
            LogLevel.Debug => "DBG",
            LogLevel.Information => "INF",
            LogLevel.Warning => "WRN",
            LogLevel.Error => "ERR",
            LogLevel.Critical => "CRT",
            _ => "NON",
        };

        return $"[{DateTime.Now:HH:mm:ss}] [{levelName}] [{categoryName}] -> {message}".EscapeMarkup();
    }
    private static IRenderable? GetScopeDataJsonText(Dictionary<string, object>? datas)
    {
        if (datas is null || datas.Count == 0) return default;

        try
        {
            var jsonString = JsonSerializer.Serialize(datas, ScopeDataJsonSerializerContext.Default.DictionaryStringObject);
            return new JsonText(jsonString);
        }
        catch (Exception ex)
        {
            return new Spectre.Console.Text(ex.Message.EscapeMarkup());
        }
    }
}