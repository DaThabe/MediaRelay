using Microsoft.Extensions.Logging;

namespace MediaRelay.Console.Logging;


internal sealed class SpectreConsoleLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new SpectreConsoleLogger(categoryName);
    public void Dispose() { }
}