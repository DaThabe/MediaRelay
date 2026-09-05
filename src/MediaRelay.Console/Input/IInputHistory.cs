namespace MediaRelay.Console.Input;


public interface IInputHistory
{
    event Action<InputMessage>? Added;

    IReadOnlyList<InputMessage> Messages { get; }
    void Add(in InputMessage message);
    void Clear();
}

public readonly record struct InputMessage
{
    public DateTime Timestamp { get; init; }
    public required string Input { get; init; }


    public InputMessage()
    {
        Timestamp = DateTime.Now;
    }
}


internal sealed class InputHistory : IInputHistory
{
    private readonly int maxHistory = 30;
    private readonly Queue<InputMessage> _history = [];

    public event Action<InputMessage>? Added;
    public IReadOnlyList<InputMessage> Messages => [.. _history];


    public void Add(in InputMessage message)
    {
        if (_history.Count >= maxHistory)
            _history.Dequeue();

        _history.Enqueue(message);
        Added?.Invoke(message);
    }
    public void Clear()
    {
        _history.Clear();
    }
}