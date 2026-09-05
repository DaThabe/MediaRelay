using Microsoft.Extensions.Logging;

namespace MediaRelay.Console.Logging;


internal sealed class ConsoleLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new SpectreConsoleLogger(categoryName);
    public void Dispose() { }
}