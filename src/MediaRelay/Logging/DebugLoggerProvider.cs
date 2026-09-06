using MediaRelay.Logging;
using Microsoft.Extensions.Logging;

namespace MediaRelay.Console.Logging;


internal sealed class DebugLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new DebugLogger(categoryName);
    public void Dispose() { }
}