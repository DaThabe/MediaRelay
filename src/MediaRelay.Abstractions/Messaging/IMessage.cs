namespace MediaRelay.Messaging;


public interface IMessage<out TContent>
{
    Guid Id { get; }
    TContent Content { get; }
}
