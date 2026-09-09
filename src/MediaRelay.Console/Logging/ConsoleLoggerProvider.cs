using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace MediaRelay.Console.Logging;


internal sealed class ConsoleLoggerProvider(IAnsiConsole ansiConsole) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new ConsoleLogger(
        categoryName: categoryName,
        ansiConsole: ansiConsole);

    public void Dispose() { }
}