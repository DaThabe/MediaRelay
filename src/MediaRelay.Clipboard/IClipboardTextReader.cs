using TextCopy;

namespace MediaRelay.Clipboard;

public interface IClipboardTextReader
{
    ValueTask<string?> ReadAsync(CancellationToken cancellationToken = default);
}


internal sealed class ClipboardTextReader : IClipboardTextReader
{
    public ValueTask<string?> ReadAsync(CancellationToken cancellationToken = default)
    {
        return new ValueTask<string?>(ClipboardService.GetTextAsync(cancellationToken));
    }
}