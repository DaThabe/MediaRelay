namespace MediaRelay.Messaging;

internal sealed record class UrlMessageRetryCounter
{
    public int CurrentCount { get; private set; }
    public required int MaxCount { get; init; }


    public static UrlMessageRetryCounter FromCount(int maxCount, int initCount = 0)
    {
        return new UrlMessageRetryCounter() { MaxCount = maxCount, CurrentCount = initCount };
    }


    public bool TryIncrement()
    {
        if (CurrentCount < MaxCount)
        {
            CurrentCount++;
            return true;
        }

        return false;
    }

    public void Recover()
    {
        CurrentCount = 0;
    }

    public override string ToString() => $"{CurrentCount}/{MaxCount}";
}