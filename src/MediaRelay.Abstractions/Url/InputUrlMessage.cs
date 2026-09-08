using MediaRelay.Messaging;
using System.Text.Json.Serialization;

namespace MediaRelay.Url;


[method: JsonConstructor]
public record class InputUrlMessage(Guid Id, Uri Content) : IMessage<Uri>
{
    public InputUrlMessage(Uri url) : this(Guid.CreateVersion7(), url) { }
}