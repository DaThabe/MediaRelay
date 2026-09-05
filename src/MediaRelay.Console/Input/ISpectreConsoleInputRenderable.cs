using Spectre.Console;
using System.Text;

namespace MediaRelay.Console.Input;


public interface ISpectreConsoleInputRenderable
{
    string ToMarkup(IReadOnlyList<InputMessage> messages);
}

internal sealed class SpectreConsoleInputRenderable : ISpectreConsoleInputRenderable
{
    private readonly Style _timestampStyle = new(Color.Grey74, null, Decoration.Dim);
    private readonly Style _messageStyle = new(Color.White, null, Decoration.Bold);


    public string ToMarkup(IReadOnlyList<InputMessage> messages)
    {
        StringBuilder sb = new();

        foreach (var i in messages)
        {
            // Time
            var timestamp = $"[{i.Timestamp:HH:mm:ss}]";
            // Message
            var message = i.Input;

            // Format
            sb.Append($"[{_timestampStyle.ToMarkup()}]{timestamp.EscapeMarkup()}[/] ");
            sb.AppendLine($" [{_messageStyle.ToMarkup()}]{message.EscapeMarkup()}[/]");
        }

        return sb.ToString();
    }
}
