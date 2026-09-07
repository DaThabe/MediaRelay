using MediaRelay.Console.Logging.Json;
using MediaRelay.Console.Logging.Text;
using Microsoft.Extensions.Logging;

namespace MediaRelay.Console.Logging;


internal sealed class LoggerProvider : ILoggerProvider
{
    public static LoggerProvider Default { get; } = new(x => new TextConsoleLogger(x));
    public static LoggerProvider Fashion { get; } = new(x => new JsonConsoleLogger(x));


    public static LoggerProvider FromStyle(LoggerStyle type)
    {
        return type switch
        {
            LoggerStyle.Json => Fashion,
            _ => Default,
        };
    }



    private readonly Func<string, ILogger> _factory;
    private LoggerProvider(Func<string, ILogger> factory) => _factory = factory;


    public ILogger CreateLogger(string categoryName) => _factory(categoryName);
    public void Dispose() { }
}
