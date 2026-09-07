using System.Diagnostics;

namespace MediaRelay.Logging;


internal interface ILoggerWriter
{
    void WriteLine(string message);
}

internal sealed class DebugLoggerWriter : ILoggerWriter
{
    private DebugLoggerWriter() { }
    public static DebugLoggerWriter Instance { get; } = new();

    public void WriteLine(string message) => Debug.WriteLine(message);
}

internal sealed class ConsoleLoggerWriter : ILoggerWriter
{
    private ConsoleLoggerWriter() { }
    public static ConsoleLoggerWriter Instance { get; } = new();

    public void WriteLine(string message) => Console.WriteLine(message);
}