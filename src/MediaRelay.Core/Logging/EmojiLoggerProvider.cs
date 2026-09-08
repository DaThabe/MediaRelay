using Microsoft.Extensions.Logging;

namespace MediaRelay.Logging;


internal sealed class EmojiLoggerProvider : ILoggerProvider
{
    private readonly ILoggerWriter _writer;
    private EmojiLoggerProvider(ILoggerWriter loggerWriter) => _writer = loggerWriter;

    public ILogger CreateLogger(string categoryName) => new EmojiLogger(categoryName, _writer);
    public void Dispose() { }


    public static EmojiLoggerProvider Debug { get; } = new(DebugLoggerWriter.Instance);
    public static EmojiLoggerProvider Console { get; } = new(ConsoleLoggerWriter.Instance);
}