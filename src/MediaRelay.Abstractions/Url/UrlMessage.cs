using MediaRelay.Messaging;
using System.Text.Json.Serialization;

namespace MediaRelay.Url;


public sealed record class UrlMessage : IMessage<Uri>
{
    [JsonIgnore]
    public MessageId Id => MessageId.FromValue(Content.ToString());
    public required Uri Content { get; init; }


    public static implicit operator UrlMessage(Uri uri) => new() { Content = uri };
    public override string ToString() => Content.ToString();
}