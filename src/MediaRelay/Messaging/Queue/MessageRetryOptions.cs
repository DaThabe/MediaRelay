namespace MediaRelay.Messaging.Queue;

internal sealed record class MessageRetryOptions
{
    public int CurrentCount { get; private set; }
    public required int MaxCount { get; init; }


    public static MessageRetryOptions FromCount(int maxCount, int initCount = 0)
    {
        return new MessageRetryOptions() { MaxCount = maxCount, CurrentCount = initCount };
    }


    public void Increment()
    {
        if (CurrentCount < MaxCount)
        {
            CurrentCount++;
            return;
        }

        throw new InvalidOperationException("已到最大重试次数");
    }

    public void Recover()
    {
        CurrentCount = 0;
    }

    public override string ToString() => $"{CurrentCount}/{MaxCount}";
}