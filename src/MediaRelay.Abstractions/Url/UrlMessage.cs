using MediaRelay.Messaging;

namespace MediaRelay.Url;


public sealed record class UrlMessage : IMessage<Uri>
{
    public MessageId Id => MessageId.FromValue(Content.ToString());
    public required Uri Content { get; init; }

    public static implicit operator UrlMessage(Uri uri) => new() { Content = uri };
}