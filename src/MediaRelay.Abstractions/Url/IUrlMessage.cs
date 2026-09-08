namespace MediaRelay.Url;

public interface IUrlMessage
{
    Uri Content { get; }
    DateTimeOffset CreatedAt { get; }

    bool TryNext(DateTimeOffset time, out IUrlMessage? next);
}