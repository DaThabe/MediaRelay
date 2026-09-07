using Microsoft.Extensions.Logging;
using System.Collections.Frozen;
using System.Diagnostics;
using System.Text.Json;

namespace MediaRelay.Logging;


internal sealed class DebugLogger(string categoryName) : ILogger
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
        var time = $"[{DateTime.Now:HH:mm:ss}]";
        var level = $"[{GetLevelString(logLevel)}]";
        var message = $"👉 {formatter(state, exception)}";
        var category = $"[{categoryName}]";
        var scopeData = GetScopeDataString(_currentScope?.ToFrozenDictionary());

        Debug.WriteLine($"{time} {level} {category} {message} {scopeData}");
        if (exception is not null) Debug.WriteLine(exception.ToString());
    }

    private static string GetLevelString(LogLevel level) => level switch
    {
        LogLevel.Trace => "👀TRC",
        LogLevel.Debug => "👽DBG",
        LogLevel.Information => "🥰INF",
        LogLevel.Warning => "😰WRN",
        LogLevel.Error => "😡ERR",
        LogLevel.Critical => "🤡CRT",
        _ => "🫥NON"
    };

    private static string GetScopeDataString(FrozenDictionary<string, object>? datas)
    {
        if (datas is null || datas.Count == 0) return string.Empty;
        return $"{{ {string.Join(", ", datas.Select(x => $"{x.Key}={x.Value}"))} }}";
    }
}