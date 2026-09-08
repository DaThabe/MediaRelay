using Microsoft.Extensions.Logging;

namespace MediaRelay.Console.Logging;


internal sealed class LoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new Logger(categoryName);
    public void Dispose() { }
}