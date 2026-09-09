using MediaRelay.Messaging;
using System.Text.Json.Serialization;

namespace MediaRelay.Url;


[method: JsonConstructor]
public record class UrlMessage(Guid Id, Uri Content) : IMessage<Uri>
{
    public UrlMessage(Uri url) : this(Guid.CreateVersion7(), url) { }
}