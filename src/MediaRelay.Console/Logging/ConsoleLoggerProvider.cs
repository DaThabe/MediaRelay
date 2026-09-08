using Microsoft.Extensions.Logging;

namespace MediaRelay.Console.Logging;


internal sealed class ConsoleLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new ConsoleLogger(categoryName);
    public void Dispose() { }


    private ConsoleLoggerProvider() { }
    public static ConsoleLoggerProvider Instance { get; } = new();
}